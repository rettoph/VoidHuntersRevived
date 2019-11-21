﻿using Guppy.Attributes;
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
            services.AddScoped<ServerShadow>();
            services.AddScoped<Sensor>(p => p.GetRequiredService<EntityCollection>().Create<Sensor>("entity:sensor"));
        }

        public void ConfigureProvider(IServiceProvider provider)
        {
            var entities = provider.GetRequiredService<EntityLoader>();

            entities.TryRegister<Sensor>("entity:sensor", "name:entity:sensor", "description:entity:sensor");
        }
    }
}
