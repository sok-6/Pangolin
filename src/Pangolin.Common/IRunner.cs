using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Common
{
    public interface IRunner
    {
        (bool, string, string) ParseSimpleCode(string simpleCode);
        void Run(string code, string arguments, IRunOptions runOptions, IEnvironmentHandleContainer environmentHandles);
        IEnumerable<string> GetAlternateStringRepresentations(string target);
        IEnumerable<string> GetAlternateIntegralRepresentations(int target);
    }
}
