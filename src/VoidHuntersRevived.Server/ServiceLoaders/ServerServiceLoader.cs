using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Server.Scenes;

namespace VoidHuntersRevived.Server.ServiceLoaders
{
    [AutoLoad]
    internal sealed class ServerServiceLoader : IServiceLoader
    {
        public void RegisterServices(ServiceCollection services)
        {
            services.AddGame<ServerVoidHuntersRevivedGame>(p => new ServerVoidHuntersRevivedGame());
            services.AddScene<GameScene>(p => new ServerGameScene(), 1);

            services.AddSetup<Settings>((s, p, c) =>
            { // Configure the server settings...
                s.Set<NetworkAuthorization>(NetworkAuthorization.Master);
            }, 1);

            services.AddSetup<NetPeerConfiguration>((config, p, c) =>
            {
                config.Port = 1337;
                config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            });
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
