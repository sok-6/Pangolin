using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pangolin.Core.TokenImplementations
{
    public class Division : ArityTwoIterableToken
    {
        private const string TOKEN_STRING = "/";

        public override string ToString() => TOKEN_STRING;

        protected override DataValue EvaluateInner(DataValue arg1, DataValue arg2)
        {
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

            if (arg2.Type == DataValueType.Numeric)
            {
                var numericArg2 = (NumericValue)arg2;

                // Check for /0
                if (numericArg1.Value == 0)
                {
                    throw new Common.PangolinException("Division by zero attempted");
                }

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

    public class Half : ArityOneIterableToken
    {
        public override string ToString() => "H";

        protected override DataValue EvaluateInner(DataValue arg)
        {
            return Division.ProcessDivision(new NumericValue(2), arg);
        }
    }
}
