using System.Collections.Generic;
using System.Linq;

namespace Pangolin.Core.DataValueImplementations
{
    public class StringValue : DataValue
    {
        public override DataValueType Type => DataValueType.String;

        public virtual string Value { get; private set; }

        public StringValue()
        {
            Value = "";
        }

        public StringValue(string value)
        {
            Value = value;
        }

        public override bool IsTruthy => Value.Length > 0;

        public override void SetIterationRequired(bool iterationRequired)
        {
            IterationRequired = iterationRequired;
        }

        public override IReadOnlyList<DataValue> IterationValues => Value.Select(c => new StringValue(c.ToString())).ToList();

        public override string ToString() => Value; // Must stay as unquoted string to allow + to work properly
    }
}
