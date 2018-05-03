using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Common.Interfaces
{
    public interface IRunOptions
    {
        bool SafeMode { get; }
        bool ArgumentParseLogging { get; }
        bool TokenisationLogging { get; }
        bool ShowExecutionPlan { get; }
        bool VerboseExecutionLogging { get; }
    }
}
