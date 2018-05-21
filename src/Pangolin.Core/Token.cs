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

        protected Exception GetInvalidArgumentTypeException(DataValueType invalidType)
        {
            return new PangolinInvalidArgumentTypeException($"Invalid argument passed to {ToString()} command - {invalidType} not supported");
        }

        #region Public factory methods

        public static Token GetStringLiteral(string literalValue) => new StringLiteral(literalValue);
        public static Token GetNumericLiteral(decimal literalValue) => new NumericLiteral(literalValue);
        public static Token GetSingleArgument(int index) => new SingleArgument(index);
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
                    // Get those which only have single token associated with it
                    .Where(t => t.ToString().Length == 1);

                _tokenMappings = new Dictionary<char, Token>();

                foreach (var t in allTokenInstances)
                {
                    _tokenMappings.Add(t.ToString()[0], t);
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
}
