﻿using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Loaders;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class ContentServiceLoader : IServiceLoader
    {
        public void ConfigureServices(ServiceCollection services)
        {
            // Register Content
            services.AddConfiguration<ContentLoader>((content, p, c) =>
            {
                content.TryRegister("ui:font:header:1", "Fonts/BiomeLight-Big");
                content.TryRegister("ui:font:header:2", "Fonts/BiomeLight-Small");
                content.TryRegister("ui:texture:logo", "Sprites/icon2alpha");

                content.TryRegister("sprite:background:1", "Sprites/background-1");
                content.TryRegister("sprite:background:2", "Sprites/background-2");
                content.TryRegister("sprite:background:3", "Sprites/background-3");
            });
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
