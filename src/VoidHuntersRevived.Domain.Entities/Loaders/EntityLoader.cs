using Guppy.Attributes;
using Guppy.Common.DependencyInjection;
using Guppy.Loaders;
using Guppy.Network;
using Guppy.Resources.Serialization.Json.Converters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common.Entities.ShipParts.Configurations;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities.Loaders
{
    public sealed class EntityLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<JsonConverter, PolymorphicJsonConverter<IShipPartComponentConfiguration>>();

            services.ConfigureCollection(manager =>
            {
                manager.AddScoped<ShipPartConfigurationService>()
                    .AddInterfaceAliases();
            });
        }
    }
}
