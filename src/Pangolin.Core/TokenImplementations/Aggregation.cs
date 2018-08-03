using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class Sum : ArityOneIterableToken
    {
        public override string ToString() => "\u03A3";

        protected override DataValue EvaluateInner(DataValue value)
        {
            // Numeric, nth triagle number if integral
            if (value.Type == DataValueType.Numeric)
            {
                var numericArg = (NumericValue)value;

                if (!numericArg.IsIntegral)
                {
                    throw new Common.PangolinInvalidArgumentTypeException($"Invalid argument type passed to \u03A3 command - non-integral numeric: {value}");
                }
                if (numericArg.Value < 0)
                {
                    throw new Common.PangolinInvalidArgumentTypeException($"Invalid argument type passed to \u03A3 command - negative numeric: {value}");
                }

                // Calculate triagle number using (n*(n+1))/2
                return new NumericValue((numericArg.IntValue * (numericArg.IntValue + 1)) / 2);
            }
            // Array, reduce on +
            else if (value.Type == DataValueType.Array)
            {
                var arrayArg = (ArrayValue)value;

                // Figure out what type the output will be
                // Array - concatenate all elements into single array
                if (arrayArg.Value.Any(v => v.Type == DataValueType.Array))
                {
                    var newArrayContents = new List<DataValue>();

                    foreach (var dv in arrayArg.Value)
                    {
                        // Add arrays as set of elements, other types as individual elements
                        if (dv.Type == DataValueType.Array)
                        {
                            newArrayContents.AddRange(dv.IterationValues);
                        }
                        else
                        {
                            newArrayContents.Add(dv);
                        }
                    }

                    return new ArrayValue(newArrayContents);
                }
                // String - convert all elements to strings and add to single string value
                else if (arrayArg.Value.Any(v => v.Type == DataValueType.String))
                {
                    var sb = new StringBuilder();

                    foreach (var dv in arrayArg.Value)
                    {
                        sb.Append(dv.ToString());
                    }

                    return new StringValue(sb.ToString());
                }
                // Numeric or empty array - simple sum, starting at 0
                else
                {
                    return new NumericValue(arrayArg.Value.Count == 0 ? 0 : arrayArg.Value.Sum(v => ((NumericValue)v).Value));
                }
            }
            // String, not implemented yet
            else
            {
                throw GetInvalidArgumentTypeException(ToString(), DataValueType.String);
            }
        }
    }
}
