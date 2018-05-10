using System;

namespace Pangolin
{
    class Program
    {
        static void Main(string[] args)
        {
            Core.Runner.Run("S*2s5 10", "", new ConsoleRunOptions());

            Console.ReadLine();
        }
    }
}
