using GalacticFighters.Client.Library;
using System;
using System.Threading;

namespace GalacticFighters.Client.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game1("localhost");
            game.Run();
        }
    }
}
