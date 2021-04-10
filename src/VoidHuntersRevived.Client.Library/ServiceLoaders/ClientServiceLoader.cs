using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Lists;
using Guppy.Network.Peers;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Windows.Library.Drivers.Entities;
using VoidHuntersRevived.Windows.Library.Drivers.Entities.Controllers;
using VoidHuntersRevived.Windows.Library.Drivers.Entities.ShipParts;
using VoidHuntersRevived.Windows.Library.Drivers.Entities.ShipParts.SpellParts;
using VoidHuntersRevived.Windows.Library.Drivers.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Windows.Library.Drivers.Players;
using VoidHuntersRevived.Windows.Library.Drivers.Services.Spells;
using VoidHuntersRevived.Windows.Library.Entities;
using VoidHuntersRevived.Windows.Library.Graphics.Effects;
using VoidHuntersRevived.Windows.Library.Graphics.Vertices;
using VoidHuntersRevived.Windows.Library.Layers;
using VoidHuntersRevived.Windows.Library.Scenes;
using VoidHuntersRevived.Windows.Library.Services;
using VoidHuntersRevived.Windows.Library.Utilities;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Special;
using VoidHuntersRevived.Library.Entities.ShipParts.SpellParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Library.Layers;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Services.Spells;

namespace VoidHuntersRevived.Windows.Library.ServiceLoaders
{
    [AutoLoad]
    public sealed class ClientServiceLoader : IServiceLoader
    {
        public void RegisterServices(ServiceCollection services)
        {
            // Configure service factories...
            services.AddFactory<Sensor>(p => new Sensor());
            services.AddFactory<PlayerNameplate>(p => new PlayerNameplate());
            services.AddFactory<DebugService>(p => new DebugService());
            services.AddFactory<ShipPartRenderService>(p => new ShipPartRenderService());
            services.AddFactory<GameLayer>(factory: p => new ClientGameLayer(), priority: 1);
            services.AddFactory<SpellLayer>(factory: p => new ClientSpellLayer(), priority: 1);
            services.AddFactory<ExplosionLayer>(factory: p => new ClientExplosionLayer(), priority: 1);
            services.AddFactory<TrailLayer>(factory: p => new ClientTrailLayer(), priority: 1);
            services.AddFactory<TrailService>(p => new TrailService());
            services.AddFactory<ServiceList<Trail>>(p => new ServiceList<Trail>());
            services.AddFactory<Trail>(p => new Trail());
            services.AddFactory<RenderTarget2DManager>(p => new RenderTarget2DManager(p.GetService<GraphicsDevice>(), p.GetService<GameWindow>()));
            services.AddFactory<PrimitiveBatch<VertexTrail, TrailEffect>>(p =>
            {
                var graphics = p.GetService<GraphicsDevice>();

                return new PrimitiveBatch<VertexTrail, TrailEffect>(
                    new TrailEffect(graphics),
                    graphics);
            });
            services.AddFactory<PrimitiveBatch<VertexExplosion, ExplosionEffect>>(p =>
            {
                var graphics = p.GetService<GraphicsDevice>();

                return new PrimitiveBatch<VertexExplosion, ExplosionEffect>(
                    new ExplosionEffect(graphics),
                    graphics);
            });

            // Configure service lifetimes...
            services.AddScoped<Sensor>();
            services.AddTransient<PlayerNameplate>();
            services.AddSingleton<DebugService>();
            services.AddScoped<ShipPartRenderService>();
            services.AddScoped<TrailService>();
            services.AddScoped<ServiceList<Trail>>();
            services.AddTransient<Trail>();
            services.AddTransient<RenderTarget2DManager>();
            services.AddScoped<PrimitiveBatch<VertexTrail, TrailEffect>>();
            services.AddScoped<PrimitiveBatch<VertexExplosion, ExplosionEffect>>();

            services.AddGame<ClientVoidHuntersRevivedGame>(p => new ClientVoidHuntersRevivedGame());
            services.AddScene<GameScene>(p => new ClientGameScene(), 1);
            services.AddScene<MainMenuScene>(p => new MainMenuScene());

            services.AddAndBindDriver<ChunkManager, ChunkManagerGraphicsDriver>(p => new ChunkManagerGraphicsDriver());
            services.AddAndBindDriver<WorldEntity, WorldEntityGraphicsDriver>(p => new WorldEntityGraphicsDriver());
            services.AddAndBindDriver<Explosion, ExplosionGraphicsDriver>(p => new ExplosionGraphicsDriver());
            services.AddAndBindDriver<ShipPart, ShipPartGraphicsDriver>(p => new ShipPartGraphicsDriver());
            services.AddAndBindDriver<SpellPart, SpellPartGraphicsDriver>(p => new SpellPartGraphicsDriver());
            services.AddAndBindDriver<Thruster, ThrusterGraphicsDriver>(p => new ThrusterGraphicsDriver());
            services.AddAndBindDriver<Player, PlayerPlayerNameplateDriver>(p => new PlayerPlayerNameplateDriver());
            services.AddAndBindDriver<UserPlayer, UserPlayerLocalDriver>(
                factory: p => new UserPlayerLocalDriver(),
                filter: (up, p) => up.User == p.GetService<ClientPeer>().CurrentUser);

            // Spells
            services.AddAndBindDriver<EnergyShieldSpell, EnergyShieldSpellGraphicsDriver>(p => new EnergyShieldSpellGraphicsDriver());
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
