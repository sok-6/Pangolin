using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class Truthify : ArityOneIterableToken
    {
        public override string ToString() => "\u00A1";
        
        protected override DataValue EvaluateInner(DataValue value)
        {
            return value.Truthify();
        }
    }

    public class UnTruthify : ArityOneIterableToken
    {
        public override string ToString() => "!";

        protected override DataValue EvaluateInner(DataValue value)
        {
            return value.IsTruthy ? DataValue.Falsey : DataValue.Truthy;
        }
    }
}
