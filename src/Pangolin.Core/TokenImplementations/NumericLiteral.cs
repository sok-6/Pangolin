using System.Collections.Generic;

namespace Pangolin.Core.TokenImplementations
{
    public class NumericLiteral : Token
    {
        public override int Arity => 0;

        public double LiteralValue { get; private set; }

        public NumericLiteral(double literalValue)
        {
            LiteralValue = literalValue;
        }

        public override DataValue Evaluate(ProgramState tokenQueue)
        {
            return new DataValueImplementations.NumericValue(LiteralValue);
        }

        public override string ToString() => LiteralValue.ToString();
    }
}
