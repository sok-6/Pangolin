using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pangolin.Core.TokenImplementations
{
    public class Modulo : ArityTwoIterableToken
    {
        public override string ToString() => "%";

        protected override DataValue EvaluateInner(DataValue arg1, DataValue arg2)
        {
            // Only defined at present between two numerics
            if (arg1.Type != DataValueType.Numeric || arg2.Type != DataValueType.Numeric)
            {
                throw GetInvalidArgumentTypeException(arg1.Type, arg2.Type);
            }

            var numericArg1 = (NumericValue)arg1;
            var numericArg2 = (NumericValue)arg2;

            return new NumericValue(numericArg2.Value % numericArg1.Value);
        }
    }
}
