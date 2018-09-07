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
    /// <summary>
    /// The base definition of a token
    /// </summary>
    public abstract class Token
    {
        public abstract int Arity { get; }
        public abstract DataValue Evaluate(ProgramState programState);
        public abstract override string ToString();

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

        private static Dictionary<char, Token> _tokenMappings = null;

        public static Token Get(char token)
        {
            if (_tokenMappings == null)
            {
                var allTokenInstances = Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    // Get those types which are Tokens and have an empty constructor
                    .Where(t => typeof(Token).IsAssignableFrom(t) && t.GetConstructor(Type.EmptyTypes) != null)
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

    }

    /// <summary>
    /// A token which accepts a function as its first argument
    /// </summary>
    public abstract class FunctionToken : Token
    {
        protected abstract int GetNestingLimit();
        protected abstract int GetNestedAmount(ProgramState programState);
        protected abstract int RetrieveFunctionArguments(ProgramState programState);
        protected abstract string GetDefaultToken(int nestingLevel);
        protected abstract void SetIterationConstants(ProgramState programState, int nestingLevel, int iterationIndex);
        protected abstract void ClearIterationConstants(ProgramState programState, int nestingLevel);
        protected abstract DataValue ProcessResults(IReadOnlyList<IterationResultContainer> results);

        protected virtual void PostExecutionAction(DataValue executionResult) { }

        public override DataValue Evaluate(ProgramState programState)
        {
            // Get the number of selects already in progress
            var nestingLevel = GetNestedAmount(programState);

            if (nestingLevel == GetNestingLimit())
            {
                throw new PangolinException($"Can't nest {ToString()} tokens more than {GetNestingLimit()} deep");
            }

            //// Get variable and index tokens
            //var variableToken = _iterationVariableTokens[alreadyRunningSelectCount];
            //var indexToken = ITERATION_INDEX_TOKENS.Substring(alreadyRunningSelectCount, 1);

            // Save current token index to return to once subsequent arguments evaluated
            var firstArgTokenIndex = programState.CurrentTokenIndex;

            // Step over 1st arg, get other arguments
            programState.StepOverNextTokenBlock();
            var iterationCount = RetrieveFunctionArguments(programState);
            
            // Save token index to return to once select token execution ended
            var endTokenIndex = programState.CurrentTokenIndex;

            // Add variable token to stack of default values
            var defaultToken = GetDefaultToken(nestingLevel);
            programState.DefaultTokenStack.Push(defaultToken);

            // Execute iteration block once per value in set
            var results = new List<IterationResultContainer>();

            for (int i = 0; i < iterationCount; i++)
            {
                // Return to the iteration block
                programState.SetCurrentTokenIndex(firstArgTokenIndex);

                // Set iteration constants
                SetIterationConstants(programState, nestingLevel, i);

                // Execute function block, add to result set
                var executionResult = programState.DequeueAndEvaluate();
                results.Add(new IterationResultContainer(i, executionResult));
                PostExecutionAction(executionResult);
            }

            // Clear iteration variable and index
            ClearIterationConstants(programState, nestingLevel);

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

    public abstract class IterationConstantToken : Token
    {
        public override int Arity => 0;

        public override DataValue Evaluate(ProgramState programState)
        {
            if (!programState.IterationFunctionConstants.TryGetValue(ToString(), out var dataValue))
            {
                throw new PangolinException($"Could not get iteration constant {ToString()} as execution not in relevant block");
            }

            return dataValue;
        }
    }
}
