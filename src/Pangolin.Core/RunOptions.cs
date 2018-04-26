using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core
{
    public class RunOptions
    {
        public bool WebExecution { get; set; }
        public bool TokenisationOutput { get; set; }
        public bool ParameterParsingOutput { get; set; }
        public bool GenerateExecutionDiagram { get; set; }
    }
}
