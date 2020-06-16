﻿using Guppy.Attributes;
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
using VoidHuntersRevived.Client.Library.Drivers.Entities;
using VoidHuntersRevived.Client.Library.Drivers.Entities.Controllers;
using VoidHuntersRevived.Client.Library.Drivers.Entities.Players;
using VoidHuntersRevived.Client.Library.Drivers.Entities.Thrusters;
using VoidHuntersRevived.Client.Library.Drivers.Scenes;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Pages;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Extensions.Utilities;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Client.Library.ServiceLoaders
{
    [AutoLoad]
    public sealed class ClientServiceLoader : IServiceLoader
    {
        public void ConfigureServices(ServiceCollection services)
        {
            services.AddConfiguration<Settings>((s, p, c) =>
            { // Configure the client settings...
                s.Set<GameAuthorization>(GameAuthorization.Partial);
            }, 1);

            services.AddScoped<FarseerCamera2D>(p => new FarseerCamera2D());
            services.AddScoped<ShipPartRenderer>(p => new ShipPartRenderer());
            services.AddScoped<Sensor>(p => new Sensor());
            services.AddScoped<TrailManager>(p => new TrailManager());
            services.AddTransient<Trail>(p => new Trail());
            services.AddTransient<TrailSegment>(p => new TrailSegment(p));

            services.AddTransient<TitlePage>(p => new TitlePage());

            services.AddGame<ClientVoidHuntersRevivedGame>(p => new ClientVoidHuntersRevivedGame());

            // Configure Drivers
            services.AddAndBindDriver<GameScene, GameSceneGraphicsDriver>(p => new GameSceneGraphicsDriver());
            services.AddAndBindDriver<WorldEntity, WorldEntityGraphicsDriver>(p => new WorldEntityGraphicsDriver());
            services.AddAndBindDriver<ShipPart, ShipPartGraphicsDriver>(p => new ShipPartGraphicsDriver());
            services.AddAndBindDriver<UserPlayer, UserPlayerLocalControllerDriver>(p => new UserPlayerLocalControllerDriver());
            services.AddAndBindDriver<ChunkManager, ChunkManagerGraphicsDriver>(p => new ChunkManagerGraphicsDriver());
            services.AddAndBindDriver<TractorBeam, TractorBeamGraphicsDriver>(p => new TractorBeamGraphicsDriver());
            services.AddAndBindDriver<Thruster, ThrusterTrailDriver>(p => new ThrusterTrailDriver());

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
                l.Font = p.GetContent<SpriteFont>("ui:font:header:1");
            });

            services.AddConfiguration<Label>("ui:header:2", (l, p, f) =>
            {
                l.Inline = true;
                l.TextAlignment = Alignment.Left;
                l.Font = p.GetContent<SpriteFont>("ui:font:header:2");
            });

            services.AddConfiguration<Component>("ui:logo", (c, p, f) =>
            {
                c.Bounds.Set(0, 0, 75, 75);
                c.Background = p.GetContent<Texture2D>("ui:texture:logo");
            });
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // Configure the console logging component...
            provider.GetService<Logger>().ConfigureConsoleLogging();

            // Increate pool size for trail segments...
            provider.GetServiceTypeDescriptor<TrailSegment>().MaxPoolSize = 5000;
        }
    }
}
