using VoidHuntersRevived.Client.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Client.Windows
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
