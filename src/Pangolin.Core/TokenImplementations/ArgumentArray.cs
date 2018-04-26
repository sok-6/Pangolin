using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class ArgumentArray : Token
    {
        public ArrayValue ArrayValue { get; private set; }

        public ArgumentArray(IReadOnlyList<DataValue> arguments)
        {
            ArrayValue = new ArrayValue(arguments);
        }

        public override DataValue Evaluate(TokenQueue tokenQueue)
        {
            return ArrayValue;
        }

        public override string ToString() => "\u00AE";
    }
}
