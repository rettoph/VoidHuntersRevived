using Guppy.Attributes;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Interfaces;
using Guppy.EntityComponent.Lists;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Components;
using VoidHuntersRevived.Library.Components.Players;
using VoidHuntersRevived.Library.Components.WorldObjects;
using VoidHuntersRevived.Library.Entities.Aether;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Services;
using Guppy.ServiceLoaders;
using Guppy.EntityComponent.DependencyInjection.Builders;
using VoidHuntersRevived.Library.Components.Aether;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class AetherWrapperServiceLoader : IServiceLoader
    {
        public void RegisterServices(AssemblyHelper assemblyHelper, ServiceProviderBuilder services)
        {

            services.RegisterService<AetherWorld>()
                .SetLifetime(ServiceLifetime.Scoped)
                .RegisterTypeFactory(factory => factory.SetDefaultConstructor<AetherWorld>());

            services.RegisterEntity<AetherBody>()
                .RegisterService(service =>
                {
                    service.SetLifetime(ServiceLifetime.Transient)
                        .RegisterTypeFactory(factory => factory.SetDefaultConstructor<AetherBody>());
                })
                .RegisterComponent<AetherBodySlaveLerpComponent>(component =>
                {
                    component.RegisterService(service =>
                    {
                        service.RegisterTypeFactory(factory => factory.SetDefaultConstructor<AetherBodySlaveLerpComponent>());
                    });
                });

            services.RegisterService<AetherFixture>()
                .SetLifetime(ServiceLifetime.Transient)
                .RegisterTypeFactory(factory => factory.SetDefaultConstructor<AetherFixture>());

            services.RegisterService<FactoryServiceList<AetherBody>>()
                .SetLifetime(ServiceLifetime.Transient)
                .RegisterTypeFactory(factory => factory.SetDefaultConstructor<FactoryServiceList<AetherBody>>());

            services.RegisterService<FactoryServiceList<AetherFixture>>()
                .SetLifetime(ServiceLifetime.Transient)
                .RegisterTypeFactory(factory => factory.SetDefaultConstructor<FactoryServiceList<AetherFixture>>());
        }
    }
}
