using Guppy;
using Guppy.Extensions;
using Guppy.Network.Extensions;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new NetPeerConfiguration("vhr")
            {
                Port = 1337,
            };

            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);


            var game = new GuppyLoader()
                .ConfigureServer(config)
                .Initialize()
                .BuildGame<ServerVoidHuntersRevivedGame>();

            game.TryStart(false);
        }
    }
}
