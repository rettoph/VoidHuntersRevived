using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Lists;
using Guppy.Network.Peers;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Client.Library.Drivers.Entities;
using VoidHuntersRevived.Client.Library.Drivers.Entities.Controllers;
using VoidHuntersRevived.Client.Library.Drivers.Entities.ShipParts;
using VoidHuntersRevived.Client.Library.Drivers.Entities.ShipParts.Special;
using VoidHuntersRevived.Client.Library.Drivers.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Client.Library.Drivers.Players;
using VoidHuntersRevived.Client.Library.Effects;
using VoidHuntersRevived.Client.Library.Entities;
using VoidHuntersRevived.Client.Library.Layers;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Client.Library.Services;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Client.Library.Utilities.Vertices;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Special;
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
            services.AddFactory<Sensor>(p => new Sensor());
            services.AddFactory<Trail>(p => new Trail());
            services.AddFactory<TrailSegment>(p => new TrailSegment());
            services.AddFactory<TrailRenderService>(p => new TrailRenderService());
            services.AddFactory<FrameableList<Trail>>(p => new FrameableList<Trail>());
            services.AddFactory<ExplosionRenderService>(p => new ExplosionRenderService());
            services.AddFactory<DebugService>(p => new DebugService());
            services.AddFactory<ShipPartRenderService>(p => new ShipPartRenderService());
            services.AddFactory<GameLayer>(factory: p => new ClientGameLayer(), priority: 1);
            services.AddFactory<RenderTarget2DManager>(p => new RenderTarget2DManager(p.GetService<GraphicsDevice>(), p.GetService<GameWindow>()));
            services.AddFactory<PrimitiveBatch<TrailVertex, TrailInterpolationEffect>>(p =>
            {
                var graphics = p.GetService<GraphicsDevice>();

                return new PrimitiveBatch<TrailVertex, TrailInterpolationEffect>(
                    new TrailInterpolationEffect(graphics),
                    graphics);
            });
            services.AddFactory<PrimitiveBatch<ExplosionVertex, ExplosionEffect>>(p =>
            {
                var graphics = p.GetService<GraphicsDevice>();

                return new PrimitiveBatch<ExplosionVertex, ExplosionEffect>(
                    new ExplosionEffect(graphics),
                    graphics);
            });

            // Configure service lifetimes...
            services.AddScoped<Sensor>();
            services.AddTransient<Trail>();
            services.AddTransient<TrailSegment>();
            services.AddScoped<TrailRenderService>();
            services.AddScoped<FrameableList<Trail>>();
            services.AddScoped<ExplosionRenderService>();
            services.AddSingleton<DebugService>();
            services.AddScoped<ShipPartRenderService>();
            services.AddTransient<RenderTarget2DManager>();
            services.AddSingleton<PrimitiveBatch<TrailVertex, TrailInterpolationEffect>>();
            services.AddSingleton<PrimitiveBatch<ExplosionVertex, ExplosionEffect>>();

            services.AddGame<ClientVoidHuntersRevivedGame>(p => new ClientVoidHuntersRevivedGame());
            services.AddScene<GameScene>(p => new ClientGameScene(), 1);
            services.AddScene<MainMenuScene>(p => new MainMenuScene());

            services.AddAndBindDriver<ChunkManager, ChunkManagerGraphicsDriver>(p => new ChunkManagerGraphicsDriver());
            services.AddAndBindDriver<WorldEntity, WorldEntityGraphicsDriver>(p => new WorldEntityGraphicsDriver());
            services.AddAndBindDriver<ShipPart, ShipPartGraphicsDriver>(p => new ShipPartGraphicsDriver());
            services.AddAndBindDriver<Thruster, ThrusterTrailsDriver>(p => new ThrusterTrailsDriver());
            services.AddAndBindDriver<ShieldGenerator, ShieldGeneratorGraphicsDriver>(p => new ShieldGeneratorGraphicsDriver());
            services.AddAndBindDriver<UserPlayer, UserPlayerLocalDriver>(
                factory: p => new UserPlayerLocalDriver(),
                filter: (up, p) => up.User == p.GetService<ClientPeer>().CurrentUser);
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
