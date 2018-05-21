using System;

namespace Pangolin
{
    class Program
    {
        static void Main(string[] args)
        {
            //Pangolin.Core.Token.Get2('+');

            Core.Runner.Run("S*2sW\u2260w2 5 10", "", new ConsoleRunOptions());
            
            Console.ReadLine();
        }
    }
}
