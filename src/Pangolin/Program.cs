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

            var simpleCode = @"`??`L^`N.`1OO@o`v|L`1O`??`L^`N.`1O`0O";

            var (success, code) = CommandLineUtilities.ParseSimpleCode(simpleCode, true);

            if (!success)
            {
                Console.WriteLine(code);
            }
            else
            {
                Core.Runner.Run(
                    code,
                    "[[1 2] [3 2]] 2",
                    new ConsoleRunOptions
                    {
                        ArgumentParseLogging = true,
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
