using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using Guppy.LayerGroups;
using Guppy.Network.Peers;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Client.Library.Drivers.Players;
using VoidHuntersRevived.Client.Library.Entities;
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
            services.AddFactory<TrailManager>(p => new TrailManager());
            services.AddFactory<Trail>(p => new Trail());
            services.AddFactory<TrailSegment>(p => new TrailSegment(p));
            services.AddFactory<DebugService>(p => new DebugService());
            services.AddFactory<ShipPartRenderService>(p => new ShipPartRenderService());

            // Configure service lifetimes...
            services.AddScoped<FarseerCamera2D>();
            services.AddScoped<Sensor>();
            services.AddScoped<TrailManager>();
            services.AddTransient<Trail>();
            services.AddTransient<TrailSegment>();
            services.AddSingleton<DebugService>();
            services.AddScoped<ShipPartRenderService>();

            services.AddGame<ClientVoidHuntersRevivedGame>(p => new ClientVoidHuntersRevivedGame());
            services.AddScene<GameScene>(p => new ClientGameScene(), 1);

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
