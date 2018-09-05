using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pangolin.Core.TokenImplementations
{
    public class Modulo : IterableToken
    {
        public override int Arity => 2;
        public override string ToString() => "%";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg1 = arguments[0];
            var arg2 = arguments[1];

            // Only defined at present between two numerics
            if (arg1.Type != DataValueType.Numeric || arg2.Type != DataValueType.Numeric)
            {
                throw GetInvalidArgumentTypeException(ToString(), arg1.Type, arg2.Type);
            }

            var numericArg1 = (NumericValue)arg1;
            var numericArg2 = (NumericValue)arg2;

            return new NumericValue(numericArg2.Value % numericArg1.Value);
        }
    }
}
