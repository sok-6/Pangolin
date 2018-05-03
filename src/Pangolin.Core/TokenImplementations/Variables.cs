using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class GetVariable : Token
    {
        private int _variableIndex;

        public override int Arity => 0;

        public GetVariable(int variableIndex)
        {
            _variableIndex = variableIndex;
        }

        public override DataValue Evaluate(ProgramState programState)
        {
            return programState.GetVariable(_variableIndex);
        }

        public override string ToString()
        {
            return ((char)('\uDD52' + _variableIndex)).ToString();
        }
    }

    public class SetVariable : Token
    {
        private const string CHAR_LIST = "\uDD38\uDD39\u2102\uDD3B\uDD3C\uDD3D\uDD3E\u210D\uDD40\uDD41";

        private int _variableIndex;

        public override int Arity => 1;

        public SetVariable(char tokenCharacter)
        {
            _variableIndex = CHAR_LIST.IndexOf(tokenCharacter);
        }

        public override DataValue Evaluate(ProgramState programState)
        {
            var argument = programState.DequeueAndEvaluate();
            programState.SetVariable(_variableIndex, argument);
            return argument;
        }

        public override string ToString()
        {
            return CHAR_LIST.Substring(_variableIndex, 1);
        }
    }
}
