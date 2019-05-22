using Guppy.Extensions;
using Guppy.Interfaces;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
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
        }

        public void Boot(IServiceProvider provider)
        {
            var driverLoader = provider.GetLoader<DriverLoader>();
            driverLoader.Register<FarseerEntity, ServerFarseerEntityDriver>();
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
