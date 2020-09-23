using Guppy;
using Guppy.Extensions;
using Guppy.Interfaces;
using Guppy.Lists;
using Guppy.Lists.Interfaces;
using Guppy.Network.Extensions;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Events;

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

            ServiceList<IService> test = null;
            test.Create<Entity>("adsad");


            var game = new GuppyLoader()
                .ConfigureServer(config)
                .Initialize()
                .BuildGame<ServerVoidHuntersRevivedGame>();

            game.TryStart(false);
        }
    }
}
