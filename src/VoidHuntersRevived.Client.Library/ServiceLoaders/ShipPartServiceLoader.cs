using Guppy.Attributes;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.DependencyInjection.Builders;
using Guppy.ServiceLoaders;
using System;
using VoidHuntersRevived.Client.Library.Components.ShipParts;
using VoidHuntersRevived.Client.Library.Services;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;

namespace VoidHuntersRevived.Client.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class ShipPartServiceLoader : IServiceLoader
    {
        public void RegisterServices(AssemblyHelper assemblyHelper, ServiceProviderBuilder services)
        {
            services.RegisterService<ShipPartRenderService>()
                .SetLifetime(ServiceLifetime.Scoped)
                .RegisterTypeFactory(factory => factory.SetDefaultConstructor<ShipPartRenderService>());

            services.RegisterComponent<ShipPartDrawComponent>()
                .SetAssignableEntityType<ShipPart>()
                .RegisterService(service =>
                {
                    service.RegisterTypeFactory(factory =>
                    {
                        factory.SetDefaultConstructor<ShipPartDrawComponent>();
                    });
                });

            services.RegisterComponent<ThrusterDrawComponent>()
                .SetAssignableEntityType<Thruster>()
                .RegisterService(service =>
                {
                    service.RegisterTypeFactory(factory =>
                    {
                        factory.SetDefaultConstructor<ThrusterDrawComponent>();
                    });
                });
        }
    }
}
