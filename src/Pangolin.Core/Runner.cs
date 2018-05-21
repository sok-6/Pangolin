using Pangolin.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using System.Linq;

namespace Pangolin.Core
{
    public static class Runner
    {
        public static void Run(string code, string arguments, IRunOptions runOptions)
        {
            // Parse arguments
            var parsedArguments = ArgumentParser.ParseArguments(arguments, runOptions.ArgumentParseLogging);

            // Tokenise code
            var tokens = Tokeniser.Tokenise(code);
            var programState = new ProgramState(parsedArguments, tokens);

            // Execute code until program end reached
            while (programState.ExecutionInProgress)
            {
                System.Console.WriteLine(programState.DequeueAndEvaluate());
            }
        }
    }
}
