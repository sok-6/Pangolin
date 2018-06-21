using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class Membership : ArityTwoIterableToken
    {
        public override string ToString() => "\u2208";

        protected override DataValue EvaluateInner(DataValue arg1, DataValue arg2)
        {
            // Array set, check if contained in entirety
            if (arg2.Type == DataValueType.Array)
            {
                return ((ArrayValue)arg2).Value.Any(a2 => EqualityBase.AreEqual(arg1, a2)) ? DataValue.Truthy : DataValue.Falsey;
            }
            // String, cast numeric to string and check for substring
            else if (arg2.Type == DataValueType.String)
            {
                if (arg1.Type == DataValueType.Array)
                {
                    throw GetInvalidArgumentTypeException(arg1.Type, arg2.Type);
                }

                var a1 = arg1.Type == DataValueType.Numeric ? ((NumericValue)arg1).Value.ToString() : ((StringValue)arg1).Value;

                return ((StringValue)arg2).Value.Contains(a1) ? DataValue.Truthy : DataValue.Falsey;
            }
            // Numeric, check if divisor
            else
            {
                if (arg1.Type != DataValueType.Numeric)
                {
                    throw GetInvalidArgumentTypeException(arg1.Type, arg2.Type);
                }

                return ((NumericValue)arg2).Value % ((NumericValue)arg1).Value == 0 ? DataValue.Truthy : DataValue.Falsey;
            }
        }
    }
}
