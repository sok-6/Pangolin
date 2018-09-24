using Pangolin.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Pangolin
{
    public class ConsoleRunOptions : IRunOptions
    {
        [Value(0, Required = false, HelpText = "The path to the file to be executed")]
        public string FilePath { get; set; }


        [Option('r', "string-representations", Default = null, Required = false, HelpText = "Get possible representations for given string")]
        public string StringRepresentations { get; set; }

        [Option('n', "int-representations", Default = null, Required = false, HelpText = "Get possible representations for given integer")]
        public int? IntRepresentations { get; set; } = null;


        [Option('c', "code", Default = null, Required = false, HelpText = "Literal code to exeucte, if no file provided")]
        public string Code { get; set; }

        [Option('a', "arguments", Default = "", Required = false, HelpText = "Argument string to be passed to code")]
        public string ArgumentString { get; set; }

        [Option('e', "pangolin-encoding", Default = false, Required = false, HelpText = "Code file uses Pangolin code page rather than UTF-16")]
        public bool PangolinEncoding { get; set; }

        [Option('i', "simple-encoding", Default = false, Required = false, HelpText = "Code uses ASCII representation")]
        public bool SimpleEncoding { get; set; }

        [Option('z', "simple-logging", Default = false, Required = false, HelpText = "Display logging for simple encoding parsing")]
        public bool SimpleLogging { get; set; }
        
        [Option('s', "safe-mode", Default = false, Required = false, HelpText = "Prohibits disk access, web access")]
        public bool SafeMode { get; set; }

        [Option('l', "argument-logging", Default = false, Required = false, HelpText = "Enables argument parsing logging")]
        public bool ArgumentParseLogging { get; set; }

        [Option('t', "tokenisation-logging", Default = false, Required = false, HelpText = "Enables tokenisation logging")]
        public bool TokenisationLogging { get; set; }

        [Option('p', "execution-plan", Default = false, Required = false, HelpText = "Displays execution plan prior to execution")]
        public bool ShowExecutionPlan { get; set; }

        [Option('x', "execution-logging", Default = false, Required = false, HelpText = "Enables execution logging")]
        public bool VerboseExecutionLogging { get; set; }
    }
}