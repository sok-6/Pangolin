namespace Pangolin.Core.DataValueImplementations
{
    public class StringValue : DataValue
    {
        public override DataValueType Type => DataValueType.String;

        private string _value;
        public string Value => _value;

        public StringValue(string value)
        {
            _value = value;
        }

        public override bool IsTruthy => _value.Length > 0;

        public override string ToString() => _value; // Must stay as unquoted string to allow + to work properly
    }
}
