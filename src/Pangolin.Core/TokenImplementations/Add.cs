using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class Add : ArityTwoIterableToken
    {
        protected override DataValue EvaluateInner(DataValue arg1, DataValue arg2)
        {
            if (arg1.Type == DataValueType.Array) // Array concatenate
            {
                var newArrayContents = new List<DataValue>(((ArrayValue)arg1).Value);

                if (arg2.Type == DataValueType.Array)
                {
                    newArrayContents.AddRange(((ArrayValue)arg2).Value);
                }
                else
                {
                    newArrayContents.Add(arg2);
                }

                return new ArrayValue(newArrayContents);
            }
            else if (arg2.Type == DataValueType.Array) // Prepend to 2nd array
            {
                var newArrayContents = new List<DataValue>();
                newArrayContents.Add(arg1);
                newArrayContents.AddRange(((ArrayValue)arg2).Value);
                return new ArrayValue(newArrayContents);
            }
            else if (arg1.Type == DataValueType.String || arg2.Type == DataValueType.String) // String concatenation
            {
                return new StringValue($"{arg1}{arg2}");
            }
            else
            {
                var value1 = ((NumericValue)arg1).Value;
                var value2 = ((NumericValue)arg2).Value;

                return new NumericValue(value1 + value2);
            }
        }

        public override string ToString() => "+";
    }
}
