using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Lists;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Components;
using VoidHuntersRevived.Library.Components.Entities.WorldObjects;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class WorldObjectServiceLoader : IServiceLoader
    {
        public void RegisterServices(GuppyServiceCollection services)
        {
            services.RegisterTypeFactory<ServiceList<IWorldObject>>(p => new ServiceList<IWorldObject>());
            services.RegisterTypeFactory<Chain>(p => new Chain());

            services.RegisterTransient<ServiceList<IWorldObject>>();
            services.RegisterTransient(Constants.ServiceConfigurationKeys.Chain);

            #region Components
            services.RegisterTypeFactory<WorldObjectChunkComponent>(p => new WorldObjectChunkComponent());
            services.RegisterTypeFactory<WorldObjectCleanWorldInfoComponent>(p => new WorldObjectCleanWorldInfoComponent());
            services.RegisterTypeFactory<WorldObjectMasterCRUDComponent>(p => new WorldObjectMasterCRUDComponent());
            services.RegisterTypeFactory<WorldObjectSlaveCRUDComponent>(p => new WorldObjectSlaveCRUDComponent());
            services.RegisterTypeFactory<ChainMasterCRUDComponent>(p => new ChainMasterCRUDComponent());
            services.RegisterTypeFactory<ChainSlaveCRUDComponent>(p => new ChainSlaveCRUDComponent());

            services.RegisterTransient<WorldObjectChunkComponent>();
            services.RegisterTransient<WorldObjectCleanWorldInfoComponent>();
            services.RegisterTransient<WorldObjectMasterCRUDComponent>();
            services.RegisterTransient<WorldObjectSlaveCRUDComponent>();
            services.RegisterTransient<ChainMasterCRUDComponent>();
            services.RegisterTransient<ChainSlaveCRUDComponent>();

            services.RegisterComponent<WorldObjectChunkComponent, IWorldObject>();
            services.RegisterComponent<WorldObjectCleanWorldInfoComponent, IWorldObject>();
            services.RegisterComponent<WorldObjectMasterCRUDComponent, IWorldObject>();
            services.RegisterComponent<WorldObjectSlaveCRUDComponent, IWorldObject>();
            services.RegisterComponent<ChainMasterCRUDComponent, Chain>();
            services.RegisterComponent<ChainSlaveCRUDComponent, Chain>();
            #endregion
        }

        public void ConfigureProvider(GuppyServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
