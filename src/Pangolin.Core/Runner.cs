using Pangolin.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using System.Linq;
using System;

namespace Pangolin.Core
{
    public static class Runner
    {
        public static void Run(string code, string arguments, IRunOptions runOptions, IEnvironmentHandleContainer environmentHandles)
        {
            // Set up log handlers
            Action<string> argumentLogHandler = NullLogger;
            if (runOptions.ArgumentParseLogging) argumentLogHandler = environmentHandles.WriteLineToArgumentLog;

            Action<string> tokenisationLogHandler = NullLogger;
            if (runOptions.TokenisationLogging) tokenisationLogHandler = environmentHandles.WriteLineToTokenLog;

            Action<string> executionPlanLogHandler = NullLogger;
            if (runOptions.ShowExecutionPlan) executionPlanLogHandler = environmentHandles.WriteLineToExecutionPlanLog;

            Action<string> outputHandler = environmentHandles.WriteToOutput;

            // Interpret program
            try
            {
                // Parse arguments
                var parsedArguments = ArgumentParser.ParseArguments(arguments, argumentLogHandler);

                // Tokenise code
                var tokens = Tokeniser.Tokenise(code, tokenisationLogHandler);
                var programState = new ProgramState(parsedArguments, tokens);

                // Execute code until program end reached
                while (programState.ExecutionInProgress)
                {
                    System.Console.WriteLine(programState.DequeueAndEvaluate());
                }
            }
            catch (Exception ex)
            {
                outputHandler($"Exception occurred, halting program run. Message: {ex.Message}");
            }
        }

        private static void NullLogger(string s) { }

        //public static void Run(string code, string arguments, IRunOptions runOptions)
        //{
        //    var allWords = System.IO.File.ReadAllLines("google-10000-english-no-swears.txt");
        //    var i = 0;

        //    var result = new List<string>();
        //    while (result.Count < 3584)
        //    {
        //        var current = allWords[i++].ToLower();

        //        if (!result.Contains(current) && current != "nt" && (current.Length > 2 || _whitelist2letter.Contains(current)))
        //        {
        //            result.Add(current);
        //        }
        //    }

        //    System.IO.File.WriteAllLines("d2.txt", result);
        //}

        //private static string[] _whitelist2letter = @"am an as at be by do dr ex go ha he hi hr if im in is it la le ma me mo mr my no of oh ok on op or ps re so to tv un up us vs we".Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
    }
}
