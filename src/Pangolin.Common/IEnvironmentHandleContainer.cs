using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Common
{
    public interface IEnvironmentHandleContainer
    {
        void WriteToOutput(string s);
        void WriteLineToArgumentLog(string s);
        void WriteLineToTokenLog(string s);
        void WriteLineToExecutionPlanLog(string s);
    }
}
