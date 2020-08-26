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
            // Register service factories...
            services.AddFactory<Settings>(p => new Settings());
            services.AddFactory<ConnectionNode>(p => new ConnectionNode());
            services.AddFactory<Logger>(p => new Logger());
            services.AddFactory<PlayerCollection>(p => new PlayerCollection());
            services.AddFactory<Chunk>(p => new Chunk());
            services.AddFactory<GameLayer>(p => new GameLayer());

            // Register services...
            services.AddScoped<Settings>();
            services.AddTransient<ConnectionNode>();
            services.AddSingleton<Logger>();
            services.AddScoped<PlayerCollection>();
            services.AddTransient<Chunk>();
            services.AddTransient<GameLayer>();


            // Register Scenes...
            services.AddScene<GameScene>(p => new GameScene());

            services.AddConfiguration<Settings>((s, p, c) =>
            { // Configure the default settings...
                s.Set<GameAuthorization>(GameAuthorization.Minimum);
            }, -10);

            // Register all default entities & their factories
            services.AddFactory<ChunkManager>(p => new ChunkManager());
            services.AddFactory<WorldEntity>(p => new WorldEntity());
            services.AddFactory<BodyEntity>(p => new BodyEntity());
            services.AddFactory<Ship>(p => new Ship());
            services.AddFactory<UserPlayer>(p => new UserPlayer());
            services.AddFactory<ShipController>(p => new ShipController());
            services.AddFactory<TractorBeam>(p => new TractorBeam());

            services.AddScoped<ChunkManager>();
            services.AddScoped<WorldEntity>();
            services.AddTransient<BodyEntity>();
            services.AddTransient<Ship>();
            services.AddTransient<UserPlayer>();
            services.AddTransient<ShipController>();
            services.AddTransient<TractorBeam>();
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
