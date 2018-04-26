using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class SingleArgument : Token
    {
        private string _argumentIndex;
        public DataValue Value { get; private set; }

        public SingleArgument(IReadOnlyList<DataValue> arguments, int index)
        {
            Value = index < arguments.Count ? arguments[index] : NumericValue.Zero;
            _argumentIndex = ((char)('\uDFD8' + index)).ToString();
        }

        public override DataValue Evaluate(TokenQueue tokenQueue)
        {
            return Value;
        }

        public override string ToString() => _argumentIndex;
    }
}
