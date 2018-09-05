using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class Reverse : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "\u042F";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            // Numeric, invert sign
            if (arg.Type == DataValueType.Numeric)
            {
                return new NumericValue(((NumericValue)arg).Value * -1);
            }
            // String, reverse
            else if (arg.Type == DataValueType.String)
            {
                return new StringValue(new String(((StringValue)arg).Value.Reverse().ToArray()));
            }
            // Array, reverse
            else
            {
                return new ArrayValue(arg.IterationValues.Reverse());
            }
        }
    }
}
