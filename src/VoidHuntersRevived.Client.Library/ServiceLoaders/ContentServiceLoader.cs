using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Windows.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class ContentServiceLoader : IServiceLoader
    {
        public void RegisterServices(ServiceCollection services)
        {
            // Register Content
            services.AddSetup<ContentService>((content, p, c) =>
            {
                content.TryRegister("debug:font", "DiagnosticsFont", 1);
                content.TryRegister("debug:font:small", "DiagnosticsFont-Small", 1);

                content.TryRegister("ui:font:header:1", "Fonts/BiomeLight-Big");
                content.TryRegister("ui:font:header:2", "Fonts/BiomeLight-Small");
                content.TryRegister("ui:texture:logo", "Sprites/icon2alpha");

                content.TryRegister("sprite:background:1", "Sprites/background-1");
                content.TryRegister("sprite:background:2", "Sprites/background-2");
                content.TryRegister("sprite:background:3", "Sprites/background-3");

                content.TryRegister("sprite:uv", "Sprites/UV");
                content.TryRegister("sprite:player-nameplate:foreground", "Sprites/player-nameplate-foreground");
                content.TryRegister("sprite:player-nameplate:background", "Sprites/player-nameplate-background");

                content.TryRegister("font:player:nameplate:1", "Fonts/PlayerNameplate-1");
            });
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
