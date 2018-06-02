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
        public override int Arity => 0;

        public ArgumentArray()
        {

        }

        public override DataValue Evaluate(ProgramState tokenQueue)
        {
            return new ArrayValue(tokenQueue.ArgumentList);
        }

        public override string ToString() => "\u00A5";
    }
}
