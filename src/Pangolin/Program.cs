using System;

namespace Pangolin
{
    class Program
    {
        static void Main(string[] args)
        {
            Core.Runner.Run("S*2sW\u2260w2 5 10", "", new ConsoleRunOptions());

            Console.ReadLine();
        }
    }
}
