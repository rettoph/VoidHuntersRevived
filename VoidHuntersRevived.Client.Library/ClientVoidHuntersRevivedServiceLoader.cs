using VoidHuntersRevived.Client.Library.Drivers;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Client.Library.Utilities;
using Guppy.Attributes;
using Guppy.Collections;
using Guppy.Interfaces;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client.Library
{
    [IsServiceLoader]
    public class ClientVoidHuntersRevivedServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<SpriteManager>();
            services.AddScoped<ServerRender>();
            services.AddTransient<DebugOverlay>(p => p.GetRequiredService<ClientWorldScene>().debugOverlay);
        }

        public void ConfigureProvider(IServiceProvider provider)
        {
            var content = provider.GetRequiredService<ContentLoader>();
            content.TryRegister("font", "font");
            content.TryRegister("thrust", "Sprites/thrust");
            content.TryRegister("com", "Sprites/com");
            content.TryRegister("bullet", "Sprites/bullet");

            content.TryRegister("sprite:ship-part:hull:triangle", "Sprites/ship-part_hull_triangle");
            content.TryRegister("sprite:ship-part:hull:square", "Sprites/ship-part_hull_square");
            content.TryRegister("sprite:ship-part:hull:hexagon", "Sprites/ship-part_hull_hexagon");
            content.TryRegister("sprite:ship-part:hull:pentagon", "Sprites/ship-part_hull_pentagon");
            content.TryRegister("sprite:ship-part:hull:beam:horizontal", "Sprites/ship-part_hull_beam_horizontal");
            content.TryRegister("sprite:ship-part:hull:beam:vertical", "Sprites/ship-part_hull_beam_vertical");
            content.TryRegister("sprite:ship-part:chassis:mosquito", "Sprites/ship-part_chassis_mosquito");
            content.TryRegister("sprite:ship-part:weapon:mass-driver", "Sprites/ship-part_weapon_mass-driver");
            content.TryRegister("sprite:ship-part:weapon:mass-driver:barrel", "Sprites/ship-part_weapon_mass-driver_barrel");
            content.TryRegister("sprite:ship-part:thruster:small", "Sprites/ship-part_thruster_small");

            var entities = provider.GetRequiredService<EntityLoader>();

            entities.TryRegister<Sensor>("sensor");
            entities.TryRegister<DebugOverlay>("debug-overlay");
        }
    }
}
