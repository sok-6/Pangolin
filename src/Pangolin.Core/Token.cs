using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using Pangolin.Core.TokenImplementations;

namespace Pangolin.Core
{
    public enum TokenType
    {
        Block,
        FunctionLed,
        TokenLed,
        Basic
    }

    /// <summary>
    /// The base definition of a token
    /// </summary>
    public abstract class Token
    {
        /// <summary>
        /// The number of arguments the token expects.
        /// <para>For TokenType.Basic/FunctionLed, this is effectively the maximum number of times Dequeue will be called to evaluate this token.</para>
        /// <para>For TokenType.TokenLed, the first argument is a token. Dequeue will be called a number of times equal to this token's Arity - 1.</para>
        /// <para>For TokenType.Block, the Arity is the number of times Dequeue will be called to evaluate the token. Any tokens in the new block are not counted. </para>
        /// </summary>
        public abstract int Arity { get; }
        public abstract DataValue Evaluate(ProgramState programState);
        public abstract override string ToString();

        public virtual TokenType Type => TokenType.Basic;

        protected static Exception GetInvalidArgumentTypeException(string tokenString, params DataValueType[] invalidTypes)
        {
            // Invalid argument types passed to x command - y,z
            return new PangolinInvalidArgumentTypeException($"Invalid argument type{(invalidTypes.Length > 1 ? "s" : "")} passed to {tokenString} command - {String.Join(",", invalidTypes)}");
        }

        protected DataValue DataValueSetToStringOrArray(IEnumerable<DataValue> dataValueSet, DataValueType destinationType)
        {
            if (destinationType == DataValueType.String)
            {
                return new StringValue(String.Join("", dataValueSet.Select(v => v.ToString())));
            }
            else
            {
                return new ArrayValue(dataValueSet);
            }
        }
        
        #region Public factory methods

        public static Token GetStringLiteral(string literalValue) => new StringLiteral(literalValue);
        public static Token GetNumericLiteral(double literalValue) => new NumericLiteral(literalValue);
        public static Token GetSingleArgument(char tokenCharacter) => new SingleArgument(tokenCharacter);
        public static Token GetGetVariable(char tokenCharacter) => new GetVariable(tokenCharacter);
        public static Token GetSetVariable(char tokenCharacter) => new SetVariable(tokenCharacter);
        public static Token GetLatestIterationConstant(int i) => new LatestIterationConstant(i);

        private static Dictionary<char, Token> _tokenMappings = null;

        public static Token Get(char token)
        {
            if (_tokenMappings == null)
            {
                var allTokenInstances = Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    // Get those types which are Tokens and have an empty constructor
                    .Where(t => typeof(Token).IsAssignableFrom(t) && t.GetConstructor(System.Type.EmptyTypes) != null)
                    // Instantiate each
                    .Select(t => (Token)Activator.CreateInstance(t))
                    // Only those with single character tokens
                    .Where(t => t.ToString().Length == 1);

                _tokenMappings = new Dictionary<char, Token>();

                foreach (var t in allTokenInstances)
                {
                    foreach (var c in t.ToString())
                    {
                        _tokenMappings.Add(c, t);
                    }
                }
            }

            Token foundToken;
            if(!_tokenMappings.TryGetValue(token, out foundToken))
            {
                throw new PangolinInvalidTokenException($"Unrecognised character in code: {token}");
            }

            return foundToken;
        }

        #endregion
    }
    
    /// <summary>
    /// A token which elevates the block level, such as loops and array builders
    /// </summary>
    public abstract class BlockToken : Token
    {
        public override TokenType Type => TokenType.Block;

        protected int _numConstantsRequired { get; private set; }

        public abstract bool BeginsBlock { get; }
        public abstract bool EndsBlock { get; }

        public BlockToken(int numConstantsRequired)
        {
            _numConstantsRequired = numConstantsRequired;
        }
    }

    /// <summary>
    /// A token which accepts a function as its first argument
    /// </summary>
    public abstract class FunctionToken : Token
    {
        public override TokenType Type => TokenType.FunctionLed;

        protected int _numConstantsRequired { get; private set; }

        protected abstract int RetrieveFunctionArguments(ProgramState programState);
        protected abstract ProgramState.IterationConstantDetails GetDefaultToken(IReadOnlyList<ProgramState.IterationConstantDetails> allocatedTokens);
        protected abstract void SetIterationConstants(ProgramState programState, IReadOnlyList<ProgramState.IterationConstantDetails> allocatedTokens, int iterationIndex);
        protected abstract DataValue ProcessResults(IReadOnlyList<IterationResultContainer> results);

        protected virtual void PostExecutionAction(DataValue executionResult) { }

        public FunctionToken(int numConstantsRequired)
        {
            _numConstantsRequired = numConstantsRequired;
        }

        public override DataValue Evaluate(ProgramState programState)
        {
            // Get the constants to use
            var allocatedIterationTokens = programState.GetIterationConstants(_numConstantsRequired);

            // Save current token index to return to once subsequent arguments evaluated
            var firstArgTokenIndex = programState.CurrentTokenIndex;

            // Step over 1st arg, get other arguments
            programState.StepOverNextFunction();
            var iterationCount = RetrieveFunctionArguments(programState);
            
            // Save token index to return to once select token execution ended
            var endTokenIndex = programState.CurrentTokenIndex;

            // Add variable token to stack of default values
            var defaultToken = GetDefaultToken(allocatedIterationTokens);
            programState.DefaultTokenStack.Push(defaultToken);

            // Execute iteration block once per value in set
            var results = new List<IterationResultContainer>();

            for (int i = 0; i < iterationCount; i++)
            {
                // Return to the iteration block
                programState.SetCurrentTokenIndex(firstArgTokenIndex);

                // Set iteration constants
                SetIterationConstants(programState, allocatedIterationTokens, i);

                // Execute function block, add to result set
                var executionResult = programState.DequeueAndEvaluate();
                results.Add(new IterationResultContainer(i, executionResult));
                PostExecutionAction(executionResult);
            }

            // Clear iteration variable and index
            ClearIterationConstants(programState, allocatedIterationTokens);

            // Remove variable token from stack of default values
            if (programState.DefaultTokenStack.Peek() != defaultToken)
            {
                throw new PangolinException($"Top of default token stack in unexpected state - {defaultToken} expected, actually {programState.DefaultTokenStack.Peek()}");
            }
            programState.DefaultTokenStack.Pop();

            // Move to end of arguments
            programState.SetCurrentTokenIndex(endTokenIndex);

            // All done, return
            return ProcessResults(results);
        }

        protected virtual void ClearIterationConstants(ProgramState programState, IReadOnlyList<ProgramState.IterationConstantDetails> allocatedTokens)
        {
            foreach (var at in allocatedTokens)
            {
                programState.ClearIterationFunctionConstant(at);
            }
        }

        protected class IterationResultContainer
        {
            public int Index { get; private set; }
            public DataValue IterationResult { get; set; }

            public IterationResultContainer(int index, DataValue iterationResult)
            {
                Index = index;
                IterationResult = iterationResult;
            }
        }
    }

    /// <summary>
    /// A token which accepts another token as its first argument
    /// </summary>
    public abstract class TokenLedToken : Token
    {
        public override TokenType Type => TokenType.TokenLed;

        protected Token _argumentToken;
        protected readonly int _requiredArgumentTokenArity = 1;

        public override DataValue Evaluate(ProgramState programState)
        {
            // Get the next token
            _argumentToken = programState.TokenList[programState.CurrentTokenIndex];

            // Check it's a token type which can be executed in isolation
            if (_argumentToken.Type != TokenType.Basic)
            {
                throw new PangolinInvalidArgumentTypeException($"Token {ToString()} can't accept {_argumentToken.ToString()} as first argument");
            }

            // Check arity
            if (_argumentToken.Arity != _requiredArgumentTokenArity)
            {
                throw new PangolinInvalidArgumentTypeException($"Token {ToString()} first argument must be arity {_requiredArgumentTokenArity} - {_argumentToken.ToString()} is arity {_argumentToken.Arity}");
            }

            // Step over token
            programState.SetCurrentTokenIndex(programState.CurrentTokenIndex + 1);

            return EvaluateInner(programState);
        }

        protected abstract DataValue EvaluateInner(ProgramState programState);
    }

    /// <summary>
    /// A token which accepts a well-defined number of arguments and produces a single result, with iteration handled by zipping
    /// </summary>
    public abstract class IterableToken : Token
    {
        public override DataValue Evaluate(ProgramState programState)
        {
            // If arity 0, just execute
            if (Arity == 0)
            {
                return EvaluateInner(new List<DataValue>());
            }

            // Dequeue the required number of arguments
            var arguments = new List<DataValue>();
            while (arguments.Count < Arity) arguments.Add(programState.DequeueAndEvaluate());

            // Compile list of argument sets for flagged iterable types
            var argumentSets = new List<List<DataValue>>();

            if (arguments[0].IterationRequired)
            {
                argumentSets.AddRange(arguments[0].IterationValues.Select(a => new List<DataValue>() { a }));
            }
            else
            {
                argumentSets.Add(new List<DataValue>() { arguments[0] });
            }

            foreach (var arg in arguments.Skip(1))
            {
                if (!arg.IterationRequired)
                {
                    foreach (var argSet in argumentSets)
                    {
                        argSet.Add(arg);
                    }
                }
                else
                {
                    var newArgumentSets = new List<List<DataValue>>();

                    foreach (var iterationArg in arg.IterationValues)
                    {
                        foreach (var argSet in argumentSets)
                        {
                            var x = new List<DataValue>(argSet) { iterationArg };

                            newArgumentSets.Add(x);
                        }
                    }

                    argumentSets = newArgumentSets;
                }
            }

            // If no iteration, execute once
            if (argumentSets.Count == 1)
            {
                return EvaluateInner(argumentSets[0]);
            }
            else
            {
                return new ArrayValue(argumentSets.Select(z => EvaluateInner(z)));
            }
        }

        protected abstract DataValue EvaluateInner(IReadOnlyList<DataValue> arguments);
    }
}
