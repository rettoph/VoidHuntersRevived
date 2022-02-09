using Guppy.Attributes;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.DependencyInjection.Builders;
using Guppy.Interfaces;
using Guppy.ServiceLoaders;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Components.ShipParts;
using VoidHuntersRevived.Client.Library.Components.Ships;
using VoidHuntersRevived.Client.Library.Services;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.Ships;

namespace VoidHuntersRevived.Client.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class ShipServiceLoader : IServiceLoader
    {
        public void RegisterServices(AssemblyHelper assemblyHelper, ServiceProviderBuilder services)
        {
            services.RegisterComponent<ShipTractorBeamDrawComponent>()
                .SetAssignableEntityType<Ship>()
                .RegisterService(service =>
                {
                    service.RegisterTypeFactory(factory =>
                    {
                        factory.SetDefaultConstructor<ShipTractorBeamDrawComponent>();
                    });
                });
        }
    }
}
