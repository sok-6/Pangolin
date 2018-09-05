using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pangolin.Core.TokenImplementations
{
    public class PrimeFactorisation : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "K";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

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

            // If 0 or 1, return empty array
            if (numericArg.IntValue < 2)
            {
                return new ArrayValue();
            }

            // Get factorisation
            var factorisation = new List<int>();
            Factorise(numericArg.IntValue, factorisation);

            return new ArrayValue(factorisation.Select(i => new NumericValue(i)));
        }

        public static IEnumerable<int> GetPrimeFactorisation(int n)
        {
            var result = new List<int>();

            Factorise(n, result);

            return result;
        }

        private static void Factorise(int value, List<int> factorisation)
        {
            // Get list of known primes
            var knownPrimes = PrimesLessThanOneMillion.PrimeList.Value;

            if (knownPrimes.Contains(value))
            {
                factorisation.Add(value);
                return;
            }

            // Trial division by known primes
            foreach (var p in knownPrimes)
            {
                if (value % p == 0)
                {
                    factorisation.Add(p);
                    Factorise(value / p, factorisation);
                    return;
                }
            }

            // Number must be larger than 1e12
            // Largest prime <1e6 is 999983 which is 5 (mod 6)
            // Use that as starting point for incremental search
            for (var trial = knownPrimes.Max(); trial * trial < value; trial += 6)
            {
                if (value % trial == 0)
                {
                    factorisation.Add(trial);
                    Factorise(value / trial, factorisation);
                    return;
                }
                if (value % (trial + 2) == 0)
                {
                    factorisation.Add((trial + 2));
                    Factorise(value / (trial + 2), factorisation);
                    return;
                }
            }

            // If it's reached here, it's a prime
            factorisation.Add(value);
            return;
        }
    }

    public class PrimesLessThanOneMillion : Token
    {
        private const int LIMIT = 1000000;

        public override int Arity => 0;

        public override DataValue Evaluate(ProgramState programState)
        {
            return new ArrayValue(PrimeList.Value.Select(p => new NumericValue(p)));
        }

        public override string ToString() => "\u1E33";

        public static Lazy<IEnumerable<int>> PrimeList = new Lazy<IEnumerable<int>>(() =>
        {
            return Palindromise_PrimesList.GetPrimesLessThan(LIMIT);
        });
    }

    public class IsPrime : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "\u1E32";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

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

            int intValue = numericArg.IntValue;
            if (intValue < 0)
            {
                throw new PangolinInvalidArgumentTypeException($"{ToString()} not defined for negative integrals - arg={arg}");
            }

            // Get list of known primes
            var knownPrimes = PrimesLessThanOneMillion.PrimeList.Value;
            var maxPrime = knownPrimes.Max();

            // If less than max known prime, check for membership
            if (intValue <= maxPrime)
            {
                return DataValue.BoolToTruthiness(knownPrimes.Contains(intValue));
            }

            // Trial division by each of the precomputed primes
            foreach (var p in knownPrimes.Where(p => p * p <= intValue))
            {
                if (intValue % p == 0)
                {
                    return DataValue.Falsey;
                }
            }

            if (Math.Sqrt(intValue) < maxPrime)
            {
                return DataValue.Truthy;
            }

            for (int i = maxPrime; i * i <= intValue; i += 6)
            {
                if (intValue % i == 0 || intValue % (i + 2) == 0)
                {
                    return DataValue.Falsey;
                }
            }

            return DataValue.Truthy;
        }
    }

    public class Palindromise_PrimesList : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "\u0416";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            // Numeric, get primes list
            if (arg.Type == DataValueType.Numeric)
            {
                var numericArg = (NumericValue)arg;
                
                // Less than 2, no primes
                if (numericArg.IntValue <= 2)
                {
                    return new ArrayValue();
                }

                // TODO: check less than 1M?

                // Get primes list
                return new ArrayValue(GetPrimesLessThan(numericArg.IntValue).Select(i => new NumericValue(i)));
            }
            // Iterable, palindromise
            else
            {
                var elements = arg.IterationValues;

                var result = new List<DataValue>();
                result.AddRange(elements);
                result.AddRange(elements.Reverse().Skip(1));

                if (arg.Type == DataValueType.String)
                {
                    return new StringValue(String.Join("", result));
                }
                else
                {
                    return new ArrayValue(result);
                }
            }
        }

        public static IEnumerable<int> GetPrimesLessThan(int n)
        {
            var candidates = Enumerable.Repeat(true, n).ToArray();
            candidates[0] = false;
            candidates[1] = false;

            for (int i = 2; i * i <= n; i++)
            {
                if (candidates[i])
                {
                    var multiple = i * 2;
                    while (multiple < n)
                    {
                        candidates[multiple] = false;
                        multiple += i;
                    }
                }
            }

            return candidates.Select((c, i) => c ? i : 0).Where(i => i > 0);
        }
    }
}
