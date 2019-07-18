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
using Guppy.Network.Drivers;

namespace VoidHuntersRevived.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var guppy = new GuppyLoader(new ConsoleLogger());
            guppy.ConfigureServer();
            guppy.Initialize();

            var game = guppy.Games.Create<VoidHuntersServerGame>();
            game.StartAsyc();
        }
    }
}
