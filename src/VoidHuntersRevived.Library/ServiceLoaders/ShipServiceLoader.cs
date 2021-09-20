using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Library.Components.Entities.Ships;
using VoidHuntersRevived.Library.Contexts.ShipParts;
using VoidHuntersRevived.Library.Contexts.Utilities;
using VoidHuntersRevived.Library.Dtos.Utilities;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Hulls;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class ShipServiceLoader : IServiceLoader
    {
        public void RegisterServices(GuppyServiceCollection services)
        {
            services.RegisterTypeFactory<ShipService>(p => new ShipService());
            services.RegisterTypeFactory<Ship>(p => new Ship());

            services.RegisterScoped<ShipService>();
            services.RegisterTransient<Ship>();

            #region Components
            // Factories
            services.RegisterTypeFactory<ShipMasterCRUDComponent>(p => new ShipMasterCRUDComponent());
            services.RegisterTypeFactory<ShipSlaveCRUDComponent>(p => new ShipSlaveCRUDComponent());

            services.RegisterTypeFactory<ShipDirectionMasterCRUDComponent>(p => new ShipDirectionMasterCRUDComponent());
            services.RegisterTypeFactory<ShipDirectionSlaveCRUDComponent>(p => new ShipDirectionSlaveCRUDComponent());

            services.RegisterTypeFactory<ShipTargetingMasterCRUDComponent>(p => new ShipTargetingMasterCRUDComponent());
            services.RegisterTypeFactory<ShipTargetingSlaveCrudComponent>(p => new ShipTargetingSlaveCrudComponent());

            // Services
            services.RegisterTransient<ShipMasterCRUDComponent>();
            services.RegisterTransient<ShipSlaveCRUDComponent>();

            services.RegisterTransient<ShipDirectionMasterCRUDComponent>(ServiceConfigurationKey.From<ShipDirectionComponent>());
            services.RegisterTransient<ShipDirectionSlaveCRUDComponent>(ServiceConfigurationKey.From<ShipDirectionComponent>());

            services.RegisterTransient<ShipTargetingMasterCRUDComponent>(ServiceConfigurationKey.From<ShipTargetingComponent>());
            services.RegisterTransient<ShipTargetingSlaveCrudComponent>(ServiceConfigurationKey.From<ShipTargetingComponent>());

            // Components
            services.RegisterComponent<ShipMasterCRUDComponent, Ship>();
            services.RegisterComponent<ShipSlaveCRUDComponent, Ship>();

            services.RegisterComponent<ShipDirectionMasterCRUDComponent, Ship>();
            services.RegisterComponent<ShipDirectionSlaveCRUDComponent, Ship>();

            services.RegisterComponent<ShipTargetingMasterCRUDComponent, Ship>();
            services.RegisterComponent<ShipTargetingSlaveCrudComponent, Ship>();
            #endregion
        }

        public void ConfigureProvider(GuppyServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
