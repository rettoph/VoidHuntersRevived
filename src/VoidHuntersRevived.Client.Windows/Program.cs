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
            using (Game1 game = new Game1())
                game.Run();
        }
    }
}
