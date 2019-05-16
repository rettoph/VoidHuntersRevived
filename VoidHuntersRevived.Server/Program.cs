using Guppy;
using Guppy.Loggers;
using Guppy.Network;
using Guppy.Network.Extensions.Guppy;
using Guppy.Network.Peers;
using System;
using Microsoft.Extensions.DependencyInjection;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Guppy.Network.Groups;

namespace VoidHuntersRevived.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var guppy = new GuppyLoader(new ConsoleLogger());
            guppy.ConfigureNetwork<ServerPeer>(Program.PeerFactory, NetworkSceneDriver.DefaultServer);
            guppy.Initialize();

            var game = guppy.Games.Create<VoidHuntersServerGame>();
            game.StartAsyc();
        }

        private static ServerPeer PeerFactory(IServiceProvider provider)
        {
            var config = provider.GetService<NetPeerConfiguration>();
            config.Port = 1337;

            return new ServerPeer(config, provider.GetService<ILogger>());
        }
    }
}
