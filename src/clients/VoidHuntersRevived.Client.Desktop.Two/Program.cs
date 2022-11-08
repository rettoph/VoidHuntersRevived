using Guppy.Attributes;
using System;
using System.Threading;
using VoidHuntersRevived.Client.Library;

namespace VoidHuntersRevived.Client.Desktop
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            Thread.Sleep(10000);

            using (var game = new Game1())
                game.Run();
        }
    }
}
