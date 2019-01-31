using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var game = new Game1();
                game.Run();
            }
            catch(Exception e)
            {
                Console.WriteLine($"{e.GetType().Name} => {e.Message}");
            }
        }
    }
}
