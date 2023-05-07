using Guppy.Attributes;
using Guppy.Common.DependencyInjection;
using Guppy.Loaders;
using Guppy.Network;
using Guppy.Network.Identity.Claims;
using Guppy.Resources.Serialization.Json;
using Guppy.Resources.Serialization.Json.Converters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Entities.ShipParts;
using VoidHuntersRevived.Common.Entities.ShipParts.Configurations;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;
using VoidHuntersRevived.Domain.Entites.Systems;
using VoidHuntersRevived.Domain.Entities.Serialization.Json.Converters;
using VoidHuntersRevived.Domain.Entities.Services;
using VoidHuntersRevived.Domain.Entities.Systems;

namespace VoidHuntersRevived.Domain.Entities.Loaders
{
    [AutoLoad]
    public sealed class EntityLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCollection(manager =>
            {
                manager.AddScoped<PilotableSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<ShipPartConfigurationService>()
                    .AddInterfaceAliases();

                manager.AddScoped<ShipPartService>()
                    .AddInterfaceAliases();

                manager.AddScoped<NodeService>()
                    .AddInterfaceAliases();

                manager.AddScoped<TreeService>()
                    .AddInterfaceAliases();

                manager.AddScoped<ChainService>()
                    .AddInterfaceAliases();

                manager.AddScoped<ShipService>()
                    .AddInterfaceAliases();

                manager.AddScoped<PilotService>()
                    .AddInterfaceAliases();

                manager.AddScoped<UserPilotMappingService>()
                    .AddInterfaceAliases();

                manager.AddScoped<RigidNodeSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<TreeSystem>()
                    .AddInterfaceAliases();

                manager.AddScoped<DestroyEntitySystem>()
                    .AddInterfaceAliases();
            });
        }
    }
}
