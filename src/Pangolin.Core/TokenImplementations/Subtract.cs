using Pangolin.Core.DataValueImplementations;

namespace Pangolin.Core.TokenImplementations
{
    public class Subtract : Token
    {
        public override int Arity => 2;

        public override DataValue Evaluate(ProgramState programState)
        {
            // Get both arguments
            var arg1 = programState.DequeueAndEvaluate();
            var arg2 = programState.DequeueAndEvaluate();

            // Only defined at present between two numerics
            if (arg1.Type != DataValueType.Numeric || arg2.Type != DataValueType.Numeric)
            {
                throw new Common.PangolinException($"Subtract token only defined for numerics - arg1.Type={arg1.Type}, arg2.Type={arg2.Type}");
            }

            var numericArg1 = (NumericValue)arg1;
            var numericArg2 = (NumericValue)arg2;

            return new NumericValue(numericArg2.Value - numericArg1.Value);
        }

        public override string ToString() => "-";
    }
}
