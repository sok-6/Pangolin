using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pangolin.Core.TokenImplementations
{
    public class Arrayify : ArityOneIterableToken
    {
        public override string ToString() => "A";

        protected override DataValue EvaluateInner(DataValue value)
        {
            // Get argument, return it wrapped in an array
            return new ArrayValue(value);
        }
    }

    public class ArrayPair : ArityTwoIterableToken
    {
        public override string ToString() => "]";

        protected override DataValue EvaluateInner(DataValue arg1, DataValue arg2)
        {
            return new ArrayValue(arg1, arg2);
        }
    }
}
