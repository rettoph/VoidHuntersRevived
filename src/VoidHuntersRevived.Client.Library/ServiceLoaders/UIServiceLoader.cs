using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Services;
using Guppy.UI.Elements;
using Guppy.UI.Entities;
using Guppy.UI.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Windows.Library.UI;
using VoidHuntersRevived.Windows.Library.UI.Pages;

namespace VoidHuntersRevived.Windows.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class UIServiceLoader : IServiceLoader
    {
        public void RegisterServices(ServiceCollection services)
        {
            services.AddFactory<MainMenuPage>(p => new MainMenuPage());
            services.AddFactory<HeaderComponent>(p => new HeaderComponent());
            services.AddFactory<FormComponent>(p => new FormComponent());

            services.AddScoped<Stage>("stage:main");
            services.AddTransient<HeaderComponent>();
            services.AddTransient<FormComponent>();
            services.AddTransient<MainMenuPage>();

            services.AddSetup<ColorService>((colors, p, c) =>
            {
                colors.TryRegister("ui:label:color:1", Color.White);
                colors.TryRegister("ui:label:color:2", new Color(0, 142, 241, 200));
                colors.TryRegister("ui:label:color:3", Color.Gray);

                colors.TryRegister("ui:input:color:1", Color.White);
                colors.TryRegister("ui:input:color:2", new Color(0, 142, 241, 200));
            });

            services.AddSetup<ContentService>((content, p, c) =>
            {
                content.TryRegister("font:ui:label:bold", "Fonts/BiomeBold-Big");
                content.TryRegister("font:ui:label:light", "Fonts/BiomeLight-Big");
                content.TryRegister("font:ui:label:small", "Fonts/Biome-Normal");
                content.TryRegister("font:ui:normal", "Fonts/Biome-Normal");
                content.TryRegister("font:ui:light", "Fonts/Biome-Light");

                content.TryRegister("sprite:ui:logo", "Sprites/icon2alpha");

                content.TryRegister("sprite:logo", "Sprites/icon2alpha");
            });

            services.AddTransient<TextElement>("ui:label:title");
            services.AddSetup<TextElement>("ui:label:title", (text, p, c) =>
            {
                text.Color[ElementState.Default] = p.GetColor("ui:label:color:1");
            });
            services.AddTransient<TextElement>("ui:label:title:small");
            services.AddSetup<TextElement>("ui:label:title:small", (text, p, c) =>
            {
                text.Color[ElementState.Default] = p.GetColor("ui:label:color:3");
                text.Font = p.GetContent<SpriteFont>("font:ui:label:small");
            });
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
