using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Lists;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Components;
using VoidHuntersRevived.Library.Components.Entities.Players;
using VoidHuntersRevived.Library.Components.Entities.WorldObjects;
using VoidHuntersRevived.Library.Entities.Aether;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class AetherWrapperServiceLoader : IServiceLoader
    {
        public void RegisterServices(GuppyServiceCollection services)
        {
            services.RegisterTypeFactory<AetherWorld>(p => new AetherWorld());
            services.RegisterTypeFactory<AetherBody>(p => new AetherBody());
            services.RegisterTypeFactory<AetherFixture>(p => new AetherFixture());
            services.RegisterTypeFactory<FactoryServiceList<AetherBody>>(p => new FactoryServiceList<AetherBody>());
            services.RegisterTypeFactory<FactoryServiceList<AetherFixture>>(p => new FactoryServiceList<AetherFixture>());

            services.RegisterScoped<AetherWorld>();
            services.RegisterTransient<AetherBody>();
            services.RegisterTransient<AetherFixture>();
            services.RegisterTransient<FactoryServiceList<AetherBody>>();
            services.RegisterTransient<FactoryServiceList<AetherFixture>>();

            #region Components
            services.RegisterTypeFactory<AetherBodySlaveLerpComponent>(p => new AetherBodySlaveLerpComponent());

            services.RegisterTransient<AetherBodySlaveLerpComponent>();

            services.RegisterComponent<AetherBodySlaveLerpComponent, AetherBody>();
            #endregion
        }

        public void ConfigureProvider(GuppyServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
