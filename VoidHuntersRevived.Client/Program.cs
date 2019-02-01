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
#if DEBUG
            var game = new Game1();
            game.Run();
#else
            try
            {
                var game = new Game1();
                game.Run();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{e.GetType().Name} => {e.Message}");
                Console.WriteLine($"{e.StackTrace.ToString()}");
            }                
#endif
        }
    }
}
