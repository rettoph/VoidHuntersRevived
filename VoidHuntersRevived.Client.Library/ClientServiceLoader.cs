using Guppy.Attributes;
using Guppy.Collections;
using Guppy.Interfaces;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;

namespace VoidHuntersRevived.Client.Library
{
    [IsServiceLoader]
    public class ClientServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<SpriteManager>();
            services.AddScoped<ServerShadow>();
            services.AddScoped<Sensor>(p => p.GetRequiredService<EntityCollection>().Create<Sensor>("entity:sensor"));
        }

        public void ConfigureProvider(IServiceProvider provider)
        {
            var content = provider.GetRequiredService<ContentLoader>();

            #region Register ShipPart Textures
            content.TryRegister("texture:entity:ship-part:hull:triangle", "Sprites/entity_ship-part_hull_triangle");
            content.TryRegister("texture:entity:ship-part:hull:square", "Sprites/entity_ship-part_hull_square");
            content.TryRegister("texture:entity:ship-part:hull:hexagon", "Sprites/entity_ship-part_hull_hexagon");

            content.TryRegister("texture:entity:ship-part:chassis:mosquito", "Sprites/entity_ship-part_chassis_mosquito");

            content.TryRegister("texture:entity:ship-part:thruster:small", "Sprites/entity_ship-part_thruster_small");
            #endregion

            var entities = provider.GetRequiredService<EntityLoader>();

            entities.TryRegister<Sensor>("entity:sensor", "name:entity:sensor", "description:entity:sensor");
        }
    }
}
