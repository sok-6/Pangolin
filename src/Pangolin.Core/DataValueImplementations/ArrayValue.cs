using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.DataValueImplementations
{
    public class ArrayValue : DataValue
    {
        private List<DataValue> _inner;
        public virtual IReadOnlyList<DataValue> Value => _inner;

        public override DataValueType Type => DataValueType.Array;

        public ArrayValue()
        {
            _inner = new List<DataValue>();
        }

        public ArrayValue(IEnumerable<DataValue> values)
        {
            _inner = new List<DataValue>(values);
        }

        public override bool IsTruthy => _inner.Count > 0;

        public override string ToString()
        {
            return $"[{String.Join(',', _inner.Select(i => i.ToString()))}]";
        }
    }
}
