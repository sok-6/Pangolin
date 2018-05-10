using System;
using System.Collections.Generic;
using System.Linq;
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

        public static class Get
        {
            public static Token StringLiteral(string literalValue) => new StringLiteral(literalValue);
            public static Token NumericLiteral(decimal literalValue) => new NumericLiteral(literalValue);

            public static Token Truthify() => new Truthify();
            public static Token UnTruthify() => new UnTruthify();
            public static Token SingleArgument(int index) => new SingleArgument(index);
            public static Token ArgumentArray() => new ArgumentArray();
            public static Token Add() => new Add();
            public static Token Multiply() => new Multiply();
            public static Token Range() => new Range();
            public static Token ReverseRange() => new ReverseRange();
            public static Token Range1() => new Range1();
            public static Token ReverseRange1() => new ReverseRange1();
            public static Token GetVariable(char tokenCharacter) => new GetVariable(tokenCharacter);
            public static Token SetVariable(char tokenCharacter) => new SetVariable(tokenCharacter);
            public static Token Equality() => new Equality();
            public static Token Inequality() => new Inequality();
            public static Token Where() => new Where();
            public static Token WhereValue() => new WhereValue();
        }
    }
}
