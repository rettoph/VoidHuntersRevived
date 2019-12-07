using Guppy.Attributes;
using Guppy.Collections;
using Guppy.Interfaces;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Drivers.Entities.Controllers;
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
            services.AddScoped<TrailManager>(p => p.GetRequiredService<EntityCollection>().Create<TrailManager>("entity:trail-manager"));
        }

        public void ConfigureProvider(IServiceProvider provider)
        {
            // Run custom global setup methods
            ChunkTextureDriver.Setup(provider);

            var content = provider.GetRequiredService<ContentLoader>();

            content.TryRegister("sprite:background:1", "Sprites/background-1");
            content.TryRegister("sprite:background:2", "Sprites/background-2");
            content.TryRegister("sprite:background:3", "Sprites/background-3");

            #region Register ShipPart Textures
            content.TryRegister("texture:entity:ship-part:hull:triangle", "Sprites/entity_ship-part_hull_triangle");
            content.TryRegister("texture:entity:ship-part:hull:square", "Sprites/entity_ship-part_hull_square");
            content.TryRegister("texture:entity:ship-part:hull:hexagon", "Sprites/entity_ship-part_hull_hexagon");

            content.TryRegister("texture:entity:ship-part:chassis:mosquito", "Sprites/entity_ship-part_chassis_mosquito");

            content.TryRegister("texture:entity:ship-part:thruster:small", "Sprites/entity_ship-part_thruster_small");

            content.TryRegister("texture:entity:ship-part:weapon:mass-driver", "Sprites/entity_ship-part_weapon_mass-driver");
            content.TryRegister("texture:entity:ammunition:projectile:mass-driver", "Sprites/entity_ammunition_projectile_mass-driver");
            #endregion

            var entities = provider.GetRequiredService<EntityLoader>();

            entities.TryRegister<Sensor>("entity:sensor", "name:entity:sensor", "description:entity:sensor");
            entities.TryRegister<TrailManager>("entity:trail-manager", "name:entity:trail-manager", "description:entity:trail-manager");
        }
    }
}
