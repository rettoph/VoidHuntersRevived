using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using Guppy.IO.Commands.Services;
using Guppy.IO.Extensions.log4net;
using Guppy.Lists;
using log4net;
using log4net.Appender;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Text;
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
        public void RegisterServices(Guppy.DependencyInjection.ServiceCollection services)
        {
            // Register service factories...
            services.AddFactory<Settings>(p => new Settings());
            services.AddFactory<ConnectionNode>(p => new ConnectionNode());
            services.AddFactory<ServiceList<Player>>(p => new ServiceList<Player>());
            services.AddFactory<Chunk>(p => new Chunk());
            services.AddFactory<GameLayer>(p => new GameLayer());
            services.AddFactory<ThreadSynchronizer>(p => new ThreadSynchronizer());

            // Register services...
            services.AddScoped<Settings>();
            services.AddTransient<ConnectionNode>();
            services.AddScoped<ServiceList<Player>>();
            services.AddTransient<Chunk>();
            services.AddTransient<GameLayer>();
            services.AddScoped<ThreadSynchronizer>("synchronizer:controller");


            // Register Scenes...
            services.AddScene<GameScene>(p => new GameScene());

            services.AddSetup<Settings>((s, p, c) =>
            { // Configure the default settings...
                s.Set<NetworkAuthorization>(NetworkAuthorization.Slave);
            }, -10);

            // Register all default entities & their factories
            services.AddFactory<ChunkManager>(p => new ChunkManager());
            services.AddFactory<WorldEntity>(p => new WorldEntity());
            services.AddFactory<BodyEntity>(p => new BodyEntity());
            services.AddFactory<Ship>(p => new Ship());
            services.AddFactory<UserPlayer>(p => new UserPlayer());
            services.AddFactory<ShipController>(p => new ShipController());
            services.AddFactory<TractorBeam>(p => new TractorBeam());
            services.AddFactory<Chain>(p => new Chain());

            services.AddScoped<ChunkManager>();
            services.AddScoped<WorldEntity>();
            services.AddTransient<BodyEntity>();
            services.AddTransient<Ship>();
            services.AddTransient<UserPlayer>();
            services.AddTransient<ShipController>();
            services.AddTransient<TractorBeam>();
            services.AddTransient<Chain>();

            services.AddSetup<ILog>((l, p, s) =>
            {
                l.SetLevel(Level.Verbose);
                l.ConfigureFileAppender($"logs\\{DateTime.Now.ToString("yyy-MM-dd")}.txt")
                    .ConfigureManagedColoredConsoleAppender(new ManagedColoredConsoleAppender.LevelColors()
                        {
                            BackColor = ConsoleColor.Red,
                            ForeColor = ConsoleColor.White,
                            Level = Level.Fatal
                        }, new ManagedColoredConsoleAppender.LevelColors()
                        {
                            ForeColor = ConsoleColor.Red,
                            Level = Level.Error
                        }, new ManagedColoredConsoleAppender.LevelColors()
                        {
                            ForeColor = ConsoleColor.Yellow,
                            Level = Level.Warn
                        }, new ManagedColoredConsoleAppender.LevelColors()
                        {
                            ForeColor = ConsoleColor.White,
                            Level = Level.Info
                        }, new ManagedColoredConsoleAppender.LevelColors()
                        {
                            ForeColor = ConsoleColor.Magenta,
                            Level = Level.Debug
                        }, new ManagedColoredConsoleAppender.LevelColors()
                        {
                            ForeColor = ConsoleColor.Cyan,
                            Level = Level.Verbose
                        });
                    });
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            var log = provider.GetService<ILog>();
            provider.GetService<CommandService>().OnExcecute += (c, a) =>
            {
                log.Debug(a);
            };
        }
    }
}
