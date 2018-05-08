using Pangolin.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin
{
    public class ConsoleRunOptions : IRunOptions
    {
        public bool SafeMode { get; set; }

        public bool ArgumentParseLogging { get; set; }

        public bool TokenisationLogging { get; set; }

        public bool ShowExecutionPlan { get; set; }

        public bool VerboseExecutionLogging { get; set; }
    }
}
