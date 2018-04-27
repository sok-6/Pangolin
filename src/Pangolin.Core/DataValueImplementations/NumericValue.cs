using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.DataValueImplementations
{
    public class NumericValue : DataValue
    {
        public override DataValueType Type => DataValueType.Numeric;

        public virtual decimal Value { get; private set; }
        public int IntValue => (int)Value;

        public NumericValue()
        {
            Value = 0;
        }

        public NumericValue(decimal value)
        {
            Value = value;
        }

        public override bool IsTruthy => Value != 0;

        public override string ToString() => Value.ToString();

        public static NumericValue Zero => new NumericValue(0);
    }
}
