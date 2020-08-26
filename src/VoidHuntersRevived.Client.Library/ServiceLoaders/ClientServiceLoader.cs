using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using Guppy.LayerGroups;
using Guppy.UI.Components;
using Guppy.UI.Enums;
using Guppy.UI.Layers;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Client.Library.Drivers.Entities;
using VoidHuntersRevived.Client.Library.Drivers.Entities.Controllers;
using VoidHuntersRevived.Client.Library.Drivers.Entities.Players;
using VoidHuntersRevived.Client.Library.Drivers.Entities.Thrusters;
using VoidHuntersRevived.Client.Library.Drivers.Layers;
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
using VoidHuntersRevived.Library.Extensions.Utilities;
using VoidHuntersRevived.Library.Layers;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Client.Library.ServiceLoaders
{
    [AutoLoad]
    public sealed class ClientServiceLoader : IServiceLoader
    {
        public void ConfigureServices(ServiceCollection services)
        {
            // Configure service factories...
            services.AddFactory<FarseerCamera2D>(p => new FarseerCamera2D());
            services.AddFactory<ShipPartRenderer>(p => new ShipPartRenderer());
            services.AddFactory<Sensor>(p => new Sensor());
            services.AddFactory<TrailManager>(p => new TrailManager());
            services.AddFactory<Trail>(p => new Trail());
            services.AddFactory<TrailSegment>(p => new TrailSegment(p));
            services.AddFactory<TitlePage>(p => new TitlePage());

            // Configure service lifetimes...
            services.AddScoped<FarseerCamera2D>();
            services.AddScoped<ShipPartRenderer>();
            services.AddScoped<Sensor>();
            services.AddScoped<TrailManager>();
            services.AddTransient<Trail>();
            services.AddTransient<TrailSegment>();
            services.AddTransient<TitlePage>();

            services.AddGame<ClientVoidHuntersRevivedGame>(p => new ClientVoidHuntersRevivedGame());

            // Configure Drivers
            services.AddAndBindDriver<GameScene, GameSceneGraphicsDriver>(p => new GameSceneGraphicsDriver());
            services.AddAndBindDriver<GameLayer, GameLayerGraphicsDriver>(p => new GameLayerGraphicsDriver());
            services.AddAndBindDriver<WorldEntity, WorldEntityGraphicsDriver>(p => new WorldEntityGraphicsDriver());
            services.AddAndBindDriver<ShipPart, ShipPartGraphicsDriver>(p => new ShipPartGraphicsDriver());
            services.AddAndBindDriver<UserPlayer, UserPlayerLocalControllerDriver>(p => new UserPlayerLocalControllerDriver());
            services.AddAndBindDriver<ChunkManager, ChunkManagerGraphicsDriver>(p => new ChunkManagerGraphicsDriver());
            services.AddAndBindDriver<TractorBeam, TractorBeamGraphicsDriver>(p => new TractorBeamGraphicsDriver());
            services.AddAndBindDriver<Thruster, ThrusterTrailDriver>(p => new ThrusterTrailDriver());

            services.AddConfiguration<Ship>((t, p, c) =>
            {
                t.LayerGroup = 10;
            });
            services.AddConfiguration<TractorBeam>((t, p, c) =>
            {
                t.LayerGroup = 10;
            });

            // Configure UI elements
            services.AddConfiguration<StageLayer>((l, p, s) =>
            {
                l.Group = new SingleLayerGroup(0);
            });

            services.AddConfiguration<Label>("ui:header:1", (l, p, s) =>
            {
                l.Inline = true;
                l.TextAlignment = Alignment.Center;
                l.Font = p.GetContent<SpriteFont>("ui:font:header:1");
            });

            services.AddConfiguration<Label>("ui:header:2", (l, p, s) =>
            {
                l.Inline = true;
                l.TextAlignment = Alignment.Left;
                l.Font = p.GetContent<SpriteFont>("ui:font:header:2");
            });

            services.AddConfiguration<Component>("ui:logo", (c, p, s) =>
            {
                c.Bounds.Set(0, 0, 75, 75);
                c.Background = p.GetContent<Texture2D>("ui:texture:logo");
            });
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // Configure the console logging component...
            provider.GetService<Logger>().ConfigureConsoleLogging();
        }
    }
}
