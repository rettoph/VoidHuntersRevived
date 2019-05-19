using Guppy.Collections;
using Guppy.Extensions;
using Guppy.Interfaces;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Entities.Drivers;
using VoidHuntersRevived.Client.Library.Layers;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;

namespace VoidHuntersRevived.Client.Library
{
    public class VoidHuntersClientServiceLoader : IServiceLoader
    {
        public void ConfigureServiceCollection(IServiceCollection services)
        {
            services.AddGame<VoidHuntersClientGame>();
            services.AddScene<VoidHuntersClientWorldScene>();
            services.AddScene<LobbyScene>();
            services.AddLayer<CameraLayer>();
            services.AddLayer<HudLayer>();

            services.AddScoped<FarseerCamera2D>();
        }

        public void Boot(IServiceProvider provider)
        {
            var entityLoader = provider.GetLoader<EntityLoader>();

            entityLoader.Register<ClientFarseerEntityDriver>("driver:farseer-entity", "name:driver:farseer-entity", "description:driver:farseer-entity");
            entityLoader.Register<ClientShipPartDriver>("driver:ship-part", "name:driver:ship-part", "description:driver:ship-part");
            entityLoader.Register<ClientPlayerDriver>("driver:player", "name:driver:player", "description:driver:player");
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
