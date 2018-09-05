using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class Truthify : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "\u00A1";
        
        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            return arg.Truthify();
        }
    }

    public class UnTruthify : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "!";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            return arg.IsTruthy ? DataValue.Falsey : DataValue.Truthy;
        }
    }
}
