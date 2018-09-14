using System;
using System.Collections.Generic;
using CommandLine;

namespace Pangolin
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Parser.Default.ParseArguments<ConsoleRunOptions>(args)
                .WithParsed<ConsoleRunOptions>(Run)
                .WithNotParsed<ConsoleRunOptions>(HandleOptionsParseErrors);

#if DEBUG
            Console.ReadLine();
#endif
        }

        static void Run(ConsoleRunOptions options)
        {
            // If string and/or int representations requested, ignore other options
            if (options.StringRepresentations != null || options.IntRepresentations != null)
            {
                if (options.StringRepresentations != null)
                {
                    Console.WriteLine($"Getting alternate representations for string \"{options.StringRepresentations}\"...");

                    foreach (var s in Core.Runner.GetAlternateStringRepresentations(options.StringRepresentations))
                    {
                        Console.WriteLine(s);
                    }
                }

                if (options.IntRepresentations != null)
                {
                    Console.WriteLine($"Getting alternate representations for integer {options.IntRepresentations}...");

                    foreach (var s in Core.Runner.GetAlternateIntegralRepresentations(options.IntRepresentations.Value))
                    {
                        Console.WriteLine(s);
                    }
                }
            }
            // Program execution
            else
            {
                // Get code
                if (options.FilePath == null && options.Code == null)
                {
                    Console.WriteLine("Error - either file path or code string must be provided");
                    return;
                }

                if (options.FilePath != null && options.Code != null)
                {
                    Console.WriteLine("Error - both file path and code string cannot be provided at same time");
                    return;
                }

                string code;
                if (options.FilePath != null)
                {
                    if (options.PangolinEncoding)
                    {
                        Console.WriteLine("Error - Pangolin file encoding not implemented yet");
                        return;
                    }
                    else
                    {
                        code = System.IO.File.ReadAllText(options.FilePath, System.Text.Encoding.Unicode);
                    }
                }
                else
                {
                    code = options.Code;
                }

                // If simple encoding, pass through parsing procedure
                if (options.SimpleEncoding)
                {
                    var (success, simpleResult) = CommandLineUtilities.ParseSimpleCode(code, true);

                    if (success)
                    {
                        code = simpleResult;
                    }
                    else
                    {
                        Console.WriteLine($"Error - {simpleResult}");
                        return;
                    }
                }

                // Execute the program
                Core.Runner.Run(
                    code,
                    options.ArgumentString,
                    options,
                    new ConsoleEnvironmentHandles());
            }
        }

        static void HandleOptionsParseErrors(IEnumerable<Error> errors)
        {
            foreach (var error in errors)
            {
                if (error.Tag != ErrorType.HelpRequestedError)
                {
                    Console.WriteLine(error.ToString());
                }
            }
        }
    }
}
