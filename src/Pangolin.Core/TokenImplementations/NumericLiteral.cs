using System.Collections.Generic;

namespace Pangolin.Core.TokenImplementations
{
    public class NumericLiteral : Token
    {
        public decimal LiteralValue { get; private set; }

        public NumericLiteral(decimal literalValue)
        {
            LiteralValue = literalValue;
        }

        public override DataValue Evaluate(TokenQueue tokenQueue)
        {
            return new DataValueImplementations.NumericValue(LiteralValue);
        }

        public override string ToString() => LiteralValue.ToString();
    }
}
