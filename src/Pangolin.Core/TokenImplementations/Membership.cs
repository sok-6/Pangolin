using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class Membership : IterableToken
    {
        public override int Arity => 2;
        public override string ToString() => "\u2208";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg1 = arguments[0];
            var arg2 = arguments[1];

            // Array set, check if contained in entirety
            if (arg2.Type == DataValueType.Array)
            {
                return DataValue.BoolToTruthiness(((ArrayValue)arg2).Value.Any(a2 => EqualityBase.AreEqual(arg1, a2)));
            }
            // String, cast numeric to string and check for substring
            else if (arg2.Type == DataValueType.String)
            {
                if (arg1.Type == DataValueType.Array)
                {
                    throw GetInvalidArgumentTypeException(ToString(), arg1.Type, arg2.Type);
                }

                var a1 = arg1.Type == DataValueType.Numeric ? ((NumericValue)arg1).Value.ToString() : ((StringValue)arg1).Value;

                return DataValue.BoolToTruthiness(((StringValue)arg2).Value.Contains(a1));
            }
            // Numeric, check if divisor
            else
            {
                if (arg1.Type != DataValueType.Numeric)
                {
                    throw GetInvalidArgumentTypeException(ToString(), arg1.Type, arg2.Type);
                }

                return DataValue.BoolToTruthiness(((NumericValue)arg2).Value % ((NumericValue)arg1).Value == 0);
            }
        }
    }
}
