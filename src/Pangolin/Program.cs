using System;

namespace Pangolin
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var (success, code) = CommandLineUtilities.ParseSimpleCode("x029", true);

            if (!success)
            {
                Console.WriteLine(code);
            }
            else
            {
                Core.Runner.Run(
                    code,
                    "",
                    new ConsoleRunOptions
                    {
                        ArgumentParseLogging = false,
                        TokenisationLogging = false,
                        ShowExecutionPlan = true,
                        VerboseExecutionLogging = true
                    },
                    new ConsoleEnvironmentHandles());
            }

            Console.ReadLine();
        }
    }
}
