using Guppy;
using Guppy.Extensions;
using Guppy.Network.Extensions;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;

namespace VoidHuntersRevived.Server
{
    class Program
    {      
        static void Main(string[] args)
        {
            var game = new GuppyLoader()
                .Initialize()
                .BuildGame<ServerVoidHuntersRevivedGame>();

            game.TryStart(false);
        }
    }
}
