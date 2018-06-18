using Pangolin.Core.DataValueImplementations;

namespace Pangolin.Core.TokenImplementations
{
    public class Division : ArityTwoIterableToken
    {
        public override string ToString() => "/";

        protected override DataValue EvaluateInner(DataValue arg1, DataValue arg2)
        {
            // Only defined at present between two numerics
            if (arg1.Type != DataValueType.Numeric || arg2.Type != DataValueType.Numeric)
            {
                throw GetInvalidArgumentTypeException(arg1.Type, arg2.Type);
            }

            var numericArg1 = (NumericValue)arg1;
            var numericArg2 = (NumericValue)arg2;

            // Check for /0
            if (numericArg1.Value == 0)
            {
                throw new Common.PangolinException("Division by zero attempted");
            }

            return new NumericValue(numericArg2.Value / numericArg1.Value);
        }
    }
}
