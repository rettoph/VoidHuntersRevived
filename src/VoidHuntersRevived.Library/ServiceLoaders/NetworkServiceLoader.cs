using Guppy.Attributes;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.DependencyInjection.Builders;
using Guppy.Interfaces;
using Guppy.ServiceLoaders;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class NetworkServiceLoader : IServiceLoader
    {
        public void RegisterServices(AssemblyHelper assemblyHelper, ServiceProviderBuilder services)
        {
            // services.RegisterTypeFactory<NetPeerConfiguration>(method: p => new NetPeerConfiguration("vhr"), priority: 1);
            // 
            // services.RegisterSetup<NetPeerConfiguration>((config, p, c) =>
            // {
            //     config.LocalAddress = IPAddress.Parse("::1");
            //     config.UseMessageRecycling = true;
            //     config.ConnectionTimeout = 30;
            // 
            //     // config.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage);
            // });
        }
    }
}
