using System;

namespace Pangolin
{
    class Program
    {
        static void Main(string[] args)
        {
            Core.Runner.Run("\u2190", "5", new ConsoleRunOptions());

            Console.ReadLine();
        }
    }
}
