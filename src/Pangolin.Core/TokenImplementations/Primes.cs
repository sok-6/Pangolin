using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;

namespace Pangolin.Core.TokenImplementations
{
    public class PrimeFactorisation : ArityOneIterableToken
    {
        public override string ToString() => "K";

        protected override DataValue EvaluateInner(DataValue arg)
        {
            // Only defined for non-negative integrals for now
            if (arg.Type != DataValueType.Numeric)
            {
                throw GetInvalidArgumentTypeException(ToString(), arg.Type);
            }

            var numericArg = (NumericValue)arg;

            if (!numericArg.IsIntegral)
            {
                throw new PangolinInvalidArgumentTypeException($"{ToString()} not defined for non-integrals - arg={arg}");
            }

            if (numericArg.IntValue < 0)
            {
                throw new PangolinInvalidArgumentTypeException($"{ToString()} not defined for negative integrals - arg={arg}");
            }

            var workingValue = numericArg.IntValue;

            // If 0 or 1, return empty array
            if (workingValue < 2)
            {
                return new ArrayValue();
            }

            throw new NotImplementedException();
        }
    }

    public class PrimesLessThanOneMillion : Token
    {
        public override int Arity => throw new NotImplementedException();

        public override DataValue Evaluate(ProgramState programState)
        {
            throw new NotImplementedException();
        }

        public override string ToString() => "k";
    }
}
