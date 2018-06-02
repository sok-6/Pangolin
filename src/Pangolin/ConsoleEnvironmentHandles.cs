using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin
{
    public class ConsoleEnvironmentHandles : Common.IEnvironmentHandleContainer
    {
        public void WriteLineToArgumentLog(string s)
        {
            Console.WriteLine("ArgumentLog: {0}", s);
        }

        public void WriteLineToExecutionPlanLog(string s)
        {
            Console.WriteLine("ExecutionPlanLog: {0}", s);
        }

        public void WriteLineToTokenLog(string s)
        {
            Console.WriteLine("TokenLog: {0}", s);
        }

        public void WriteToOutput(string s)
        {
            Console.Write(s);
        }
    }
}
