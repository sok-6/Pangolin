using System;

namespace Pangolin
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Core.Runner.Run(
                "S*2sW\u2260w2 5 10", 
                "", 
                new ConsoleRunOptions
                {
                    ArgumentParseLogging = true,
                    TokenisationLogging = true,
                    ShowExecutionPlan = true,
                    VerboseExecutionLogging = true
                }, 
                new ConsoleEnvironmentHandles());
            
            Console.ReadLine();
        }
    }
}
