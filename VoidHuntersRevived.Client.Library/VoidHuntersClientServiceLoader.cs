using Guppy.Collections;
using Guppy.Extensions;
using Guppy.Interfaces;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Drivers;
using VoidHuntersRevived.Client.Library.Layers;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Client.Library
{
    public class VoidHuntersClientServiceLoader : IServiceLoader
    {
        public void ConfigureServiceCollection(IServiceCollection services)
        {
            services.AddScoped<FarseerCamera2D>();

            services.AddGame<VoidHuntersClientGame>();
            services.AddScene<VoidHuntersClientWorldScene>();
            services.AddScene<LobbyScene>();
            services.AddLayer<CameraLayer>();
            services.AddLayer<HudLayer>();
        }

        public void Boot(IServiceProvider provider)
        {
            var driverLoader = provider.GetLoader<DriverLoader>();
            driverLoader.Register<FarseerEntity, ClientFarseerEntityDriver>();
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
