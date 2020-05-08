using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using Guppy.LayerGroups;
using Guppy.Loaders;
using Guppy.UI.Components;
using Guppy.UI.Enums;
using Guppy.UI.Layers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Pages;
using VoidHuntersRevived.Client.Library.Scenes;

namespace VoidHuntersRevived.Client.Library.ServiceLoaders
{
    [AutoLoad]
    public sealed class ClientServiceLoader : IServiceLoader
    {
        public void ConfigureServices(ServiceCollection services)
        {
            services.AddTransient<TitlePage>(p => new TitlePage());

            services.AddGame<ClientVoidHuntersRevivedGame>(p => new ClientVoidHuntersRevivedGame());

            services.AddScene<MainMenuScene>(p => new MainMenuScene());

            // Configure UI elements
            services.AddConfiguration<StageLayer>((l, p, c) =>
            {
                l.Group = new SingleLayerGroup(0);
                return l;
            });

            services.AddConfiguration<Label>("ui:header:1", (l, p, f) =>
            {
                l.Inline = true;
                l.TextAlignment = Alignment.Center;
                l.Bounds.Height = 100;
                l.Font = p.GetContent<SpriteFont>("ui:font:header:1");
            });

            // Register Content
            services.AddConfiguration<ContentLoader>((content, p, c) =>
            {
                content.TryRegister("ui:font:header:1", "Fonts/BiomeLight-Big");
            });
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
