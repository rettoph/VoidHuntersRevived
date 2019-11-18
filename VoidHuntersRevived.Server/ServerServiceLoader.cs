using Guppy.Attributes;
using Guppy.Interfaces;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Server
{
    [IsServiceLoader]
    public class ServerServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // throw new NotImplementedException();
        }

        public void ConfigureProvider(IServiceProvider provider)
        {
            var config = provider.GetService<NetPeerConfiguration>();
            config.Port = 1337;
            // config.AutoFlushSendQueue = false;
        }
    }
}
