using GalacticFighters.Client.Library.Drivers;
using GalacticFighters.Client.Library.Entities;
using GalacticFighters.Client.Library.Scenes;
using GalacticFighters.Client.Library.Utilities;
using Guppy.Attributes;
using Guppy.Collections;
using Guppy.Interfaces;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Client.Library
{
    [IsServiceLoader]
    public class ClientGalacticFightersServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ServerRender>();
            services.AddTransient<DebugOverlay>(p => p.GetRequiredService<ClientGalacticFightersWorldScene>().debugOverlay);
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
            content.TryRegister("sprite:ship-part:chassis:mosquito", "Sprites/ship-part_chassis_mosquito");

            var entities = provider.GetRequiredService<EntityLoader>();

            entities.TryRegister<Sensor>("sensor");
            entities.TryRegister<DebugOverlay>("debug-overlay");
        }
    }
}
