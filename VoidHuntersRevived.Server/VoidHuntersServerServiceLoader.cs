using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Loaders;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Server.Drivers;
using VoidHuntersRevived.Server.Scenes;

namespace VoidHuntersRevived.Server
{
    public class VoidHuntersServerServiceLoader : IServiceLoader
    {
        public void ConfigureServiceCollection(IServiceCollection services)
        {
            services.AddGame<VoidHuntersServerGame>();
            services.AddScene<VoidHuntersServerWorldScene>();

            services.AddDriver<Player, ServerPlayerDriver>();
            services.AddDriver<UserPlayer, ServerUserPlayerDriver>();
            services.AddDriver<FarseerEntity, ServerFarseerEntityDriver>();
            services.AddDriver<Ship, ServerShipDriver>();
            services.AddDriver<ShipPart, ServerShipPartDriver>();
        }

        public void Boot(IServiceProvider provider)
        {
            var config = provider.GetService<NetPeerConfiguration>();
            config.Port = 1337;
        }

        public void PreInitialize(IServiceProvider provider)
        {
            // throw new NotImplementedException();
        }

        public void Initialize(IServiceProvider provider)
        {
            // throw new NotImplementedException();
        }

        public void PostInitialize(IServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
