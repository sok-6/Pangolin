using Pangolin.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pangolin.Core
{
    public static class Runner
    {
        public static void Run(string code, string arguments, IRunOptions runOptions)
        {
            // Parse arguments
            var parsedArguments = ArgumentParser.ParseArguments(arguments, runOptions.ArgumentParseLogging);

            // Tokenise code
            var programState = Tokeniser.Tokenise(code, parsedArguments);

            // Execute code until program end reached
            while (programState.ExecutionInProgress)
            {
                System.Console.WriteLine(programState.DequeueAndEvaluate());
            }
        }
    }
}
