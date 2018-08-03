using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public abstract class ParityCheckTokenBase : ArityOneIterableToken
    {
        protected DataValue ProcessParity(DataValue arg, bool evenTest)
        {
            if (arg.Type == DataValueType.Numeric)
            {
                var numericArg = (NumericValue)arg;

                if (!numericArg.IsIntegral)
                {
                    return DataValue.Falsey;
                }
                else
                {
                    return DataValue.BoolToTruthiness((numericArg.IntValue % 2 == 0) ^ !evenTest);
                }
            }
            else
            {
                // String or array, use length
                return DataValue.BoolToTruthiness((arg.IterationValues.Count % 2 == 0) ^ !evenTest);
            }
        }
    }

    public class IsEven : ParityCheckTokenBase
    {
        public override string ToString() => "\u1EB8";

        protected override DataValue EvaluateInner(DataValue arg)
        {
            return ProcessParity(arg, true);
        }
    }

    public class IsOdd : ParityCheckTokenBase
    {
        public override string ToString() => "\u1ECC";

        protected override DataValue EvaluateInner(DataValue arg)
        {
            return ProcessParity(arg, false);
        }
    }
}
