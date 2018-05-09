using System;
using System.Collections.Generic;
using System.Text;

namespace Pangolin.Core.TokenImplementations
{
    public class Where : Token
    {
        public override int Arity => 2;

        public override DataValue Evaluate(ProgramState programState)
        {
            // Check if already in where block
            if (programState.WhereIndex != null)
            {
                throw new Pangolin.Common.PangolinException("Where token encountered in where block");
            }

            // Make note of current index to return to 
            var whereIndex = programState.CurrentTokenIndex;

            // Find index of 
            throw new NotImplementedException();
        }

        public override string ToString() => "W";
    }
}
