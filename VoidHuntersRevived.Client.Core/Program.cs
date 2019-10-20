using VoidHuntersRevived.Client.Library;
using System;
using System.Threading;

namespace VoidHuntersRevived.Client.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Host:");
            var game = new Game1("localhost");
            game.Run();
        }
    }
}
