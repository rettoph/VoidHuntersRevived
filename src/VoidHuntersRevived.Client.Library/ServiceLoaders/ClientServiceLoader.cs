using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using Guppy.LayerGroups;
using Guppy.Lists;
using Guppy.Network.Peers;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Client.Library.Drivers.Entities;
using VoidHuntersRevived.Client.Library.Drivers.Entities.Controllers;
using VoidHuntersRevived.Client.Library.Drivers.Entities.ShipParts;
using VoidHuntersRevived.Client.Library.Drivers.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Client.Library.Drivers.Players;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Layers;
using VoidHuntersRevived.Client.Library.Scenes;
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
        public void RegisterServices(ServiceCollection services)
        {
            // Configure service factories...
            services.AddFactory<FarseerCamera2D>(p => new FarseerCamera2D());
            services.AddFactory<Sensor>(p => new Sensor());
            services.AddFactory<Trail>(p => new Trail());
            services.AddFactory<TrailSegment>(p => new TrailSegment());
            services.AddFactory<TrailService>(p => new TrailService());
            services.AddFactory<ServiceList<Trail>>(p => new ServiceList<Trail>());
            services.AddFactory<DebugService>(p => new DebugService());
            services.AddFactory<ShipPartRenderService>(p => new ShipPartRenderService());
            services.AddFactory<GameLayer>(factory: p => new ClientGameLayer(), priority: 1);

            // Configure service lifetimes...
            services.AddScoped<FarseerCamera2D>();
            services.AddScoped<Sensor>();
            services.AddTransient<Trail>();
            services.AddTransient<TrailSegment>();
            services.AddSingleton<TrailService>();
            services.AddSingleton<ServiceList<Trail>>();
            services.AddSingleton<DebugService>();
            services.AddScoped<ShipPartRenderService>();

            services.AddGame<ClientVoidHuntersRevivedGame>(p => new ClientVoidHuntersRevivedGame());
            services.AddScene<GameScene>(p => new ClientGameScene(), 1);

            services.AddAndBindDriver<ChunkManager, ChunkManagerGraphicsDriver>(p => new ChunkManagerGraphicsDriver());
            services.AddAndBindDriver<WorldEntity, WorldEntityGraphicsDriver>(p => new WorldEntityGraphicsDriver());
            services.AddAndBindDriver<ShipPart, ShipPartGraphicsDriver>(p => new ShipPartGraphicsDriver());
            services.AddAndBindDriver<Thruster, ThrusterTrailsDriver>(p => new ThrusterTrailsDriver());
            services.AddAndBindDriver<UserPlayer, UserPlayerLocalDriver>(
                factory: p => new UserPlayerLocalDriver(),
                filter: (up, p) => up.User == p.GetService<ClientPeer>().CurrentUser);

            services.AddSetup<Ship>((t, p, c) =>
            {
                t.LayerGroup = 10;
            });
            services.AddSetup<TractorBeam>((t, p, c) =>
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
