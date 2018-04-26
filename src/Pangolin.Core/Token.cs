using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pangolin.Core.TokenImplementations;

namespace Pangolin.Core
{
    public abstract class Token
    {
        public abstract DataValue Evaluate(TokenQueue tokenQueue);
        public abstract override string ToString();

        public static class Get
        {
            public static Token StringLiteral(string literalValue) => new StringLiteral(literalValue);
            public static Token NumericLiteral(decimal literalValue) => new NumericLiteral(literalValue);

            public static Token Truthify() => new Truthify();
            public static Token UnTruthify() => new UnTruthify();
            public static Token SingleArgument(IReadOnlyList<DataValue> arguments, int index) => new SingleArgument(arguments, index);
            public static Token ArgumentArray(IReadOnlyList<DataValue> arguments) => new ArgumentArray(arguments);
            public static Token Add() => new Add();
        }
    }
}
