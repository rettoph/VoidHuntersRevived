﻿using GalacticFighters.Client.Library.Drivers;
using GalacticFighters.Client.Library.Entities;
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
        }

        public void ConfigureProvider(IServiceProvider provider)
        {
            var content = provider.GetRequiredService<ContentLoader>();
            content.TryRegister("font", "font");
            content.TryRegister("thrust", "Sprites/thrust");
            content.TryRegister("com", "Sprites/com");

            var entities = provider.GetRequiredService<EntityLoader>();

            entities.TryRegister<Sensor>("sensor");
        }
    }
}
