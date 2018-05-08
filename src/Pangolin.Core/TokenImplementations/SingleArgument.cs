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
        public override int Arity => 0;

        public int ArgumentIndex { get; private set; }

        public SingleArgument(int index)
        {
            ArgumentIndex = index; 
        }

        public override DataValue Evaluate(ProgramState programState)
        {
            return ArgumentIndex < programState.ArgumentList.Count ? programState.ArgumentList[ArgumentIndex] : NumericValue.Zero;
        }

        public override string ToString() => ((char)('\uDFD8' + ArgumentIndex)).ToString();
    }
}
