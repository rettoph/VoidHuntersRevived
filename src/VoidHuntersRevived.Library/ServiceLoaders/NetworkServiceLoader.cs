using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class NetworkServiceLoader : IServiceLoader
    {
        public void RegisterServices(GuppyServiceCollection services)
        {
            services.RegisterTypeFactory<NetPeerConfiguration>(method: p => new NetPeerConfiguration("vhr"), priority: 1);

            services.RegisterSetup<NetPeerConfiguration>((config, p, c) =>
            {
                config.LocalAddress = IPAddress.Parse("::1");
                config.UseMessageRecycling = true;
                config.ConnectionTimeout = 30;

                config.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage);
            });
        }

        public void ConfigureProvider(GuppyServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
