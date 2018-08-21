using System;

namespace Pangolin
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            //foreach (var s in Core.Runner.GetAlternateIntegralRepresentations(540))
            //{
            //    Console.WriteLine(s);
            //}

            var simpleCode = "`N.S`S.S?`C-j]i-1-iL`0Ot`s.`0O";

            var (success, code) = CommandLineUtilities.ParseSimpleCode(simpleCode, true);

            if (!success)
            {
                Console.WriteLine(code);
            }
            else
            {
                Core.Runner.Run(
                    code,
                    "CODE-GOLF",
                    new ConsoleRunOptions
                    {
                        ArgumentParseLogging = false,
                        TokenisationLogging = true,
                        ShowExecutionPlan = true,
                        VerboseExecutionLogging = true
                    },
                    new ConsoleEnvironmentHandles());
            }

            Console.ReadLine();
        }
    }
}
