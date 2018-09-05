using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pangolin.Core.TokenImplementations
{
    public class Division : IterableToken
    {
        private const string TOKEN_STRING = "/";
        
        public override int Arity => 2;
        public override string ToString() => TOKEN_STRING;

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg1 = arguments[0];
            var arg2 = arguments[1];

            return ProcessDivision(arg1, arg2);
        }

        public static DataValue ProcessDivision(DataValue arg1, DataValue arg2)
        {
            // Only defined at present if 1st argument numeric
            if (arg1.Type != DataValueType.Numeric)
            {
                throw GetInvalidArgumentTypeException(TOKEN_STRING, arg1.Type, arg2.Type);
            }

            var numericArg1 = (NumericValue)arg1;

            // Check for /0
            if (numericArg1.Value == 0)
            {
                throw new Common.PangolinException("Division by zero attempted");
            }

            if (arg2.Type == DataValueType.Numeric)
            {
                var numericArg2 = (NumericValue)arg2;

                return new NumericValue(numericArg2.Value / numericArg1.Value);
            }
            else
            {
                // Split defined for integral parts only
                if (!numericArg1.IsIntegral)
                {
                    throw new PangolinException($"Can't divide into non-integral number of parts - {numericArg1.Value} requested");
                }

                var pieceCount = numericArg1.IntValue;
                var elements = arg2.IterationValues;

                var pieceSizes = Enumerable.Range(0, pieceCount)
                    .Select(i => (elements.Count() / pieceCount) + ((i < elements.Count % pieceCount) ? 1 : 0));

                var result = new List<IEnumerable<DataValue>>();

                foreach (var size in pieceSizes)
                {
                    result.Add(elements.Skip(result.Sum(r => r.Count())).Take(size));
                }

                if (arg2.Type == DataValueType.String)
                {
                    // String - concatenate each into string before return
                    return new ArrayValue(result.Select(x => new StringValue(String.Join("", x.Select(v => ((StringValue)v).Value)))));
                }
                else
                {
                    return new ArrayValue(result.Select(x => new ArrayValue(x)));
                }
            }
        }
    }

    public class Half : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "H";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            return Division.ProcessDivision(new NumericValue(2), arg);
        }
    }

    public class IntegerDivision : IterableToken
    {
        public override int Arity => 2;
        public override string ToString() => "\u00F7";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg1 = arguments[0];
            var arg2 = arguments[1];

            if (arg2.Type == DataValueType.Numeric)
            {
                // Only defined if 1st argument numeric for now
                if (arg1.Type != DataValueType.Numeric)
                {
                    throw GetInvalidArgumentTypeException(ToString(), arg1.Type, arg2.Type);
                }

                var divisor = ((NumericValue)arg1).Value;

                // Check for /0
                if (divisor == 0)
                {
                    throw new Common.PangolinException("Division by zero attempted");
                }

                return new NumericValue((int)(((NumericValue)arg2).Value / divisor));
            }
            // String/Array, split into equal sized parts
            else
            {
                var elements = arg2.IterationValues;

                var result = new List<IEnumerable<DataValue>>();
                var blockSizes = new List<int>();

                // If 1st arg is numeric, split into that sized blocks
                if (arg1.Type == DataValueType.Numeric)
                {
                    var numericArg1 = (NumericValue)arg1;

                    if (!numericArg1.IsIntegral)
                    {
                        throw new PangolinInvalidArgumentTypeException($"For {ToString()}, if 1st argument is numeric, it must be integral - arg1={arg1}, arg2={arg2}");
                    }

                    if (numericArg1.IntValue == 0)
                    {
                        throw new PangolinInvalidArgumentTypeException($"For {ToString()}, if 1st argument is numeric, it must be non-zero - arg1={arg1}, arg2={arg2}");
                    }

                    blockSizes.Add(numericArg1.IntValue);
                }
                // Array divisor, only defined if all elements are ints
                else if (arg1.Type == DataValueType.Array)
                {
                    // All inner values must be integrals
                    var arrayValues = ((ArrayValue)arg1).Value;

                    if (arrayValues.Count == 0)
                    {
                        throw new PangolinInvalidArgumentTypeException($"For {ToString()}, if 1st argument is array, it must be populated - arg1={arg1}, arg2={arg2}");
                    }

                    if (!arrayValues.All(a => a.Type == DataValueType.Numeric && ((NumericValue)a).IsIntegral))
                    {
                        throw new PangolinInvalidArgumentTypeException($"For {ToString()}, if 1st argument is array, all elements of that array must be integral - arg1={arg1}, arg2={arg2}");
                    }

                    var numericValues = arrayValues.Cast<NumericValue>();

                    // At least one value must be non-0
                    if (numericValues.All(n => n.IntValue == 0))
                    {
                        throw new PangolinInvalidArgumentTypeException($"For {ToString()}, if 1st argument is array, at least one element must be non-zero - arg1={arg1}, arg2={arg2}");
                    }

                    // Set block sizes
                    blockSizes.AddRange(numericValues.Select(n => n.IntValue));
                }
                // String divisor, not defined
                else
                {
                    throw GetInvalidArgumentTypeException(ToString(), arg1.Type, arg2.Type);
                }

                // Get blocks
                while (result.Sum(r => r.Count()) < elements.Count)
                {
                    var blockSize = blockSizes[result.Count % blockSizes.Count];

                    var nextBlock = elements.Skip(result.Sum(r => r.Count())).Take(Math.Abs(blockSize));

                    if (blockSize < 0)
                    {
                        nextBlock = nextBlock.Reverse();
                    }

                    result.Add(nextBlock);
                }

                if (arg2.Type == DataValueType.String)
                {
                    // String - concatenate each into string before return
                    return new ArrayValue(result.Select(x => new StringValue(String.Join("", x.Select(v => ((StringValue)v).Value)))));
                }
                else
                {
                    return new ArrayValue(result.Select(x => new ArrayValue(x)));
                }
            }
        }
    }
}
