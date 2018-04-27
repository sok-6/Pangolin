using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class StringLiteral : Token
    {
        public override int Arity => 0;

        public string LiteralValue { get; private set; }

        public StringLiteral(string literalValue)
        {
            LiteralValue = literalValue;
        }

        public override DataValue Evaluate(TokenQueue tokenQueue)
        {
            return new DataValueImplementations.StringValue(LiteralValue);
        }

        public override string ToString() => $"\"{LiteralValue}\"";
    }
}
