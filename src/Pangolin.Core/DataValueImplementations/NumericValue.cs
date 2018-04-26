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

        private decimal _value;
        public decimal Value => _value;
        public int IntValue => (int)_value;

        public NumericValue(decimal value)
        {
            _value = value;
        }

        public override bool IsTruthy => _value != 0;

        public override string ToString() => _value.ToString();

        public static NumericValue Zero => new NumericValue(0);
    }
}
