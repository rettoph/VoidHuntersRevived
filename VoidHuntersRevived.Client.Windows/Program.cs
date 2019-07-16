using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoidHuntersRevived.Client.Library;

namespace VoidHuntersRevived.Client.Windows
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(5000);

            var game = new Game1();
            game.Run();
        }
    }
}
