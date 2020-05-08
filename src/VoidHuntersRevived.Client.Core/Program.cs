using System;
using VoidHuntersRevived.Client.Library;

namespace VoidHuntersRevived.Client.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Game1 game = new Game1())
                game.Run();
        }
    }
}
