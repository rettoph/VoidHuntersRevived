using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Collections;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Layers;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class MainServiceLoader : IServiceLoader
    {
        public void ConfigureServices(ServiceCollection services)
        {
            services.AddScoped<Settings>(p => new Settings());
            services.AddScene<GameScene>(p => new GameScene());
            services.AddTransient<ConnectionNode>(p => new ConnectionNode());
            services.AddSingleton<Logger>(p => new Logger());
            services.AddScoped<PlayerCollection>(p => new PlayerCollection());
            services.AddTransient<Chunk>(p => new Chunk());

            services.AddConfiguration<Settings>((s, p, c) =>
            { // Configure the default settings...
                s.Set<GameAuthorization>(GameAuthorization.None);
            });

            // Register all default layers
            services.AddTransient<GameLayer>(p => new GameLayer());

            // Register all default entities
            services.AddScoped<ChunkManager>(p => new ChunkManager());
            services.AddScoped<WorldEntity>(p => new WorldEntity());
            services.AddEntity<BodyEntity>(p => new BodyEntity());
            services.AddEntity<Ship>(p => new Ship());
            services.AddEntity<UserPlayer>(p => new UserPlayer());
            services.AddEntity<ShipController>(p => new ShipController());
            services.AddEntity<TractorBeam>(p => new TractorBeam());
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
