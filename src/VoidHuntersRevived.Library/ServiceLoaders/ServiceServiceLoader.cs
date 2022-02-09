using Guppy.Attributes;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.DependencyInjection.Builders;
using Guppy.Interfaces;
using Guppy.ServiceLoaders;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class ServiceServiceLoader : IServiceLoader
    {
        public void RegisterServices(AssemblyHelper assemblyHelper, ServiceProviderBuilder services)
        {
            services.RegisterService<ShipPartService>()
                .SetLifetime(ServiceLifetime.Scoped)
                .RegisterTypeFactory(factory => factory.SetDefaultConstructor<ShipPartService>());

            services.RegisterService<ChainService>()
                .SetLifetime(ServiceLifetime.Scoped)
                .RegisterTypeFactory(factory => factory.SetDefaultConstructor<ChainService>());
        }
    }
}
