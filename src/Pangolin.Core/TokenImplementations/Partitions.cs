using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class MultiplicativePartitions_PowerSet : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "#";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            // Numeric, multiplicative partitions
            if (arg.Type == DataValueType.Numeric)
            {
                var numericArg = (NumericValue)arg;

                if (!numericArg.IsIntegral)
                {
                    throw new PangolinInvalidArgumentTypeException($"{ToString()} not defined for non-integrals - arg={arg}");
                }
                if (numericArg.IntValue < 0)
                {
                    throw new PangolinInvalidArgumentTypeException($"{ToString()} not defined for negative integrals - arg={arg}");
                }

                // If less than 2, no prime factorisation available
                if (numericArg.IntValue < 2)
                {
                    return new ArrayValue(new ArrayValue(numericArg));
                }

                // Get prime factorisation
                var factorisation = PrimeFactorisation.GetPrimeFactorisation(numericArg.IntValue);
                var result = new List<IEnumerable<int>>();

                throw new NotImplementedException("Multiplicative partitions are hard :/");
            }
            // Iterable, power set
            else
            {
                var elements = arg.IterationValues;
                var results = new List<IEnumerable<DataValue>>();

                // Order binary inclusion to product correct order output
                var orderedbinaryInclusion = Enumerable.Range(0, (int)Math.Pow(2, elements.Count))
                    .Select(i => new { V = i, B = Enumerable.Range(0, elements.Count).Select(b => (i & (1 << b)) != 0).Reverse().ToArray() })
                    .OrderBy(x => x.B.Count(b => b)).ThenByDescending(x => x.V);

                foreach (var x in orderedbinaryInclusion)
                {
                    results.Add(elements.Where((v, i) => x.B[i]));
                }

                if (arg.Type == DataValueType.String)
                {
                    return new ArrayValue(results.Select(r => new StringValue(String.Join("", r))));
                }
                else
                {
                    return new ArrayValue(results.Select(r => new ArrayValue(r)));
                }
            }
        }
    }

    public class Multiples_Endings : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "E";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            throw new NotImplementedException();
        }
    }
}
