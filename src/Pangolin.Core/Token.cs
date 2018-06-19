using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Pangolin.Common;
using Pangolin.Core.TokenImplementations;

namespace Pangolin.Core
{
    public abstract class Token
    {
        public abstract int Arity { get; }
        public abstract DataValue Evaluate(ProgramState programState);
        public abstract override string ToString();

        protected Exception GetInvalidArgumentTypeException(params DataValueType[] invalidTypes)
        {
            return new PangolinInvalidArgumentTypeException($"Invalid argument type{(invalidTypes.Length > 1 ? "s" : "")} passed to {ToString()} command - {String.Join(",", invalidTypes)}");
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

    public abstract class ArityOneIterableToken : Token
    {
        public override int Arity => 1;

        public override DataValue Evaluate(ProgramState programState)
        {
            // Get argument
            var arg = programState.DequeueAndEvaluate();

            if (arg.IterationRequired)
            {
                return new DataValueImplementations.ArrayValue(arg.IterationValues.Select(a => EvaluateInner(a)));
            }
            else
            {
                return EvaluateInner(arg);
            }
        }

        protected abstract DataValue EvaluateInner(DataValue value);
    }

    public abstract class ArityTwoIterableToken : Token
    {
        public override int Arity => 2;

        public override DataValue Evaluate(ProgramState tokenQueue)
        {
            // Get two arguments
            var arg1 = tokenQueue.DequeueAndEvaluate();
            var arg2 = tokenQueue.DequeueAndEvaluate();

            if (arg1.IterationRequired)
            {
                if (arg2.IterationRequired)
                {
                    // Zip them
                    return new DataValueImplementations.ArrayValue(arg1.IterationValues.Zip(arg2.IterationValues, (a1,a2) => EvaluateInner(a1, a2)));
                }
                else
                {
                    return new DataValueImplementations.ArrayValue(arg1.IterationValues.Select(a1 => EvaluateInner(a1, arg2)));
                }
            }
            else
            {
                if (arg2.IterationRequired)
                {
                    return new DataValueImplementations.ArrayValue(arg2.IterationValues.Select(a2 => EvaluateInner(arg1, a2)));
                }
                else
                {
                    return EvaluateInner(arg1, arg2);
                }
            }
        }

        protected abstract DataValue EvaluateInner(DataValue arg1, DataValue arg2);
    }
}
