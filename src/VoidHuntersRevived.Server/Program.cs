using Guppy;
using Guppy.Extensions;
using Guppy.Network.Extensions;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using Guppy.Extensions.System;

namespace VoidHuntersRevived.Server
{
    class Program
    {      
        static void Main(string[] args)
        {
            Console.WriteLine("entity:ship-part:drone-bay".xxHash());
            var game = new GuppyLoader()
                .Initialize()
                .BuildGame<ServerVoidHuntersRevivedGame>();

            game.TryStart(false);
        }
    }
}
