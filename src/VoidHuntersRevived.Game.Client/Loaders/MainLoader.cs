using Guppy.Attributes;
using Guppy.Common.DependencyInjection;
using Guppy.Loaders;
using Guppy.MonoGame;
using Guppy.MonoGame.Utilities.Cameras;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Game.Client.GameComponents;

namespace VoidHuntersRevived.Game.Client.Loaders
{
    [AutoLoad]
    internal sealed class MainLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGuppy<MainMenuGuppy>();
            services.AddGuppy<MultiplayerGameGuppy>();

            services.ConfigureCollection(manager =>
            {
                manager.AddScoped<ScreenComponent>()
                    .AddAlias<IGameComponent>();

                manager.AddScoped<Camera2D>()
                    .AddAlias<Camera>();
            });
        }
    }
}
