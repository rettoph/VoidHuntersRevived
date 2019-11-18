using Guppy;
using Guppy.Network;
using Guppy.Network.Peers;
using System;
using Microsoft.Extensions.DependencyInjection;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Guppy.Utilities.Loggers;
using Guppy.Network.Extensions;

namespace VoidHuntersRevived.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new GuppyLoader()
                .ConfigureLogger<ConsoleLogger>()
                .ConfigureNetwork("vhr")
                .Initialize()
                .BuildGame<ServerGame>();

            game.TryStartAsync();
        }
    }
}
