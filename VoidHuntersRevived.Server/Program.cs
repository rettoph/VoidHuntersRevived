using Microsoft.Xna.Framework;
using System;
using System.Threading;
using VoidHuntersRevived.Library;

namespace VoidHuntersRevived.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            GameTime gameTime;
            DateTime start = DateTime.Now;
            DateTime old = DateTime.Now;
            DateTime now;

            VoidHuntersRevivedServerGame game = new VoidHuntersRevivedServerGame(new VoidHuntersRevivedLogger());

            while(true)
            {
                now = DateTime.Now;

                gameTime = new GameTime(
                    now.Subtract(start),
                    now.Subtract(old));

                old = now;

                game.Update(gameTime);

                Thread.Sleep(16);
            }
        }
    }
}
