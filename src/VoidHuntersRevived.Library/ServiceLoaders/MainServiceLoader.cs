using Guppy;
using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
using Guppy.Extensions.DependencyInjection;
using Guppy.Extensions.System;
using Guppy.Interfaces;
using Guppy.IO.Commands;
using Guppy.IO.Commands.Services;
using Guppy.IO.Extensions.log4net;
using Guppy.Lists;
using Guppy.Network.Peers;
using Guppy.Utilities;
using Lidgren.Network;
using log4net;
using log4net.Appender;
using log4net.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
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
            services.AddFactory<Explosion>(p => new Explosion());

            // Register services...
            services.AddScoped<Settings>();
            services.AddTransient<ConnectionNode>();
            services.AddScoped<ServiceList<Player>>();
            services.AddTransient<Chunk>();
            services.AddTransient<GameLayer>();
            services.AddTransient<Explosion>();


            // Register Scenes...
            services.AddScene<GameScene>(p => new GameScene());

            services.AddSetup<Settings>((s, p, c) =>
            { // Configure the default settings...
                s.Set<NetworkAuthorization>(NetworkAuthorization.Slave);
            }, -10);

            // Register all default entities & their factories
            services.AddFactory<ChunkManager>(p => new ChunkManager());
            services.AddFactory<ServiceList<NetworkEntity>>(p => new ServiceList<NetworkEntity>());
            services.AddFactory<WorldEntity>(p => new WorldEntity());
            services.AddFactory<BodyEntity>(p => new BodyEntity());
            services.AddFactory<Ship>(p => new Ship());
            services.AddFactory<UserPlayer>(p => new UserPlayer());
            services.AddFactory<ComputerPlayer>(p => new ComputerPlayer());
            services.AddFactory<ShipController>(p => new ShipController());
            services.AddFactory<TractorBeam>(p => new TractorBeam());
            services.AddFactory<Chain>(p => new Chain());

            services.AddScoped<ChunkManager>();
            services.AddScoped<ServiceList<NetworkEntity>>();
            services.AddScoped<WorldEntity>();
            services.AddTransient<BodyEntity>();
            services.AddTransient<Ship>();
            services.AddTransient<UserPlayer>();
            services.AddTransient<ComputerPlayer>();
            services.AddTransient<ShipController>();
            services.AddTransient<TractorBeam>();
            services.AddTransient<Chain>();

            services.AddSetup<Entity>((e, p, c) =>
            {
                if(c.Lifetime == ServiceLifetime.Scoped)
                {
                    p.GetService<EntityList>().Then(entities =>
                    {
                        if (!entities.Contains(e))
                            entities.TryAdd(e);
                    });
                }
            }, 15);

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

            services.AddSetup<NetPeerConfiguration>((config, p, c) =>
            {
                config.LocalAddress = IPAddress.Parse("::1");
                // EnumHelper.GetValues<NetIncomingMessageType>().ForEach(mt => config.EnableMessageType(mt));
            });

            // services.AddSetup<Peer>((peer, p, c) =>
            // {
            //     EnumHelper.GetValues<NetIncomingMessageType>().ForEach(mt =>
            //     {
            //         peer.MessageTypeDelegates[mt] += m =>
            //         {
            //             switch (m.MessageType)
            //             {
            //                 case NetIncomingMessageType.WarningMessage:
            //                 case NetIncomingMessageType.ErrorMessage:
            //                 case NetIncomingMessageType.DebugMessage:
            //                 case NetIncomingMessageType.VerboseDebugMessage:
            //                 case NetIncomingMessageType.Error:
            //                     Console.WriteLine($"{m.MessageType}: {m.ReadString()}");
            //                     break;
            //                 default:
            //                     Console.WriteLine(m.MessageType);
            //                     break;
            //             }
            //         };
            //     });
            // });
        }

        public void ConfigureProvider(Guppy.DependencyInjection.ServiceProvider provider)
        {
            var log = provider.GetService<ILog>();
            provider.GetService<CommandService>().OnExcecuted += (arguments, responses) =>
            {
                log.Debug(arguments);

                responses.ForEach(response =>
                {
                    switch (response.Type)
                    {
                        case CommandResponseType.Success:
                            log.Info(response.ToString());
                            break;
                        case CommandResponseType.Warning:
                            log.Warn(response.ToString());
                            break;
                        case CommandResponseType.Error:
                            log.Error(response.ToString());
                            break;
                    }
                });
            };
        }
    }
}
