using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using Guppy.LayerGroups;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Client.Library.Drivers.Entities;
using VoidHuntersRevived.Client.Library.Drivers.Entities.Controllers;
using VoidHuntersRevived.Client.Library.Drivers.Entities.Players;
using VoidHuntersRevived.Client.Library.Drivers.Entities.Thrusters;
using VoidHuntersRevived.Client.Library.Drivers.Layers;
using VoidHuntersRevived.Client.Library.Drivers.Scenes;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Services;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Client.Library.Utilities.Cameras;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Library.Layers;
using VoidHuntersRevived.Library.Scenes;

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
            services.AddFactory<ButtonService>(p => new ButtonService());
            services.AddFactory<DebugService>(p => new DebugService());
            services.AddFactory<InputCommandService>(p => new InputCommandService());

            // Configure service lifetimes...
            services.AddScoped<FarseerCamera2D>();
            services.AddScoped<ShipPartRenderer>();
            services.AddScoped<Sensor>();
            services.AddScoped<TrailManager>();
            services.AddTransient<Trail>();
            services.AddTransient<TrailSegment>();
            services.AddSingleton<ButtonService>();
            services.AddSingleton<DebugService>();
            services.AddSingleton<InputCommandService>(autoBuild: true);

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
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
