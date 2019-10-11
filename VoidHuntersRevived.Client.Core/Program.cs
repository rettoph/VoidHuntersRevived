using GalacticFighters.Client.Library;
using System;
using System.Threading;

namespace GalacticFighters.Client.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Host:");
            var game = new Game1(Console.ReadLine());
            game.Run();
        }
    }
}
