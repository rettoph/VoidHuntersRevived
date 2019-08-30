using Guppy.Attributes;
using Guppy.Interfaces;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Server.ServiceLoaders
{
    [IsServiceLoader]
    public sealed class ServerServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void ConfigureProvider(IServiceProvider provider)
        {
            var configuration = provider.GetRequiredService<NetPeerConfiguration>();
            configuration.Port = 1337;
            configuration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
        }
    }
}
