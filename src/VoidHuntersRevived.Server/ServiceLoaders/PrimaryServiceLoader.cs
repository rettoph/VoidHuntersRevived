using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Network.Enums;
using Guppy.Utilities;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Server.Scenes;

namespace VoidHuntersRevived.Server.ServiceLoaders
{
    [AutoLoad]
    internal sealed class PrimaryServiceLoader : IServiceLoader
    {
        public void RegisterServices(GuppyServiceCollection services)
        {
            services.RegisterTypeFactory<PrimaryGame>(p => new ServerPrimaryGame());
            services.RegisterTypeFactory<PrimaryScene>(p => new ServerPrimaryScene());

            services.RegisterSetup<Settings>((s, p, c) =>
            { // Configure the server settings...
                s.Set<NetworkAuthorization>(NetworkAuthorization.Master);
            }, 1);

            services.RegisterSetup<NetPeerConfiguration>((config, p, c) =>
            {
                config.Port = 1337;
                config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            });
        }

        public void ConfigureProvider(GuppyServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
