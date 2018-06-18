using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pangolin.Core.TokenImplementations
{
    public class UnTruthify : ArityOneIterableToken
    {
        public override string ToString() => "!";

        protected override DataValue EvaluateInner(DataValue value)
        {
            return value.IsTruthy ? DataValue.Falsey : DataValue.Truthy;
        }
    }
}
