using Guppy.Attributes;
using System;
using VoidHuntersRevived.Client.Library;

namespace VoidHuntersRevived.Client.Desktop
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Game1())
                game.Run();
        }
    }
}
