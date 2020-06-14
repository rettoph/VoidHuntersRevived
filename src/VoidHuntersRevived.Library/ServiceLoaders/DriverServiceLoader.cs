using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Drivers.Scenes;
using VoidHuntersRevived.Library.Drivers.Entities;
using VoidHuntersRevived.Library.Drivers.Entities.Players;
using VoidHuntersRevived.Library.Drivers.Scenes;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    /// <summary>
    /// Service loader primarily in charge of adding base drivers
    /// into the game.
    /// </summary>
    [AutoLoad]
    internal sealed class DriverServiceLoader : IServiceLoader
    {
        public void ConfigureServices(ServiceCollection services)
        {
            services.AddAndBindDriver<GameScene, GameSceneFullAuthorityNetworkDriver>(p => new GameSceneFullAuthorityNetworkDriver());
            services.AddAndBindDriver<GameScene, GameScenePartialAuthrotityNetworkDriver>(p => new GameScenePartialAuthrotityNetworkDriver());

            services.AddAndBindDriver<Player, PlayerFullAuthorizationNetworkDriver>(p => new PlayerFullAuthorizationNetworkDriver());
            services.AddAndBindDriver<Player, PlayerPartialAuthorizationNetworkDriver>(p => new PlayerPartialAuthorizationNetworkDriver());
            services.AddAndBindDriver<UserPlayer, UserPlayerFullAuthorizationNetworkDriver>(p => new UserPlayerFullAuthorizationNetworkDriver());
            services.AddAndBindDriver<UserPlayer, UserPlayerPartialAuthorizationNetworkDriver>(p => new UserPlayerPartialAuthorizationNetworkDriver());
            services.AddAndBindDriver<WorldEntity, WorldEntityFullAuthorizationNetworkDriver>(p => new WorldEntityFullAuthorizationNetworkDriver());
            services.AddAndBindDriver<WorldEntity, WorldEntityPartialAuthorizationNetworkDriver>(p => new WorldEntityPartialAuthorizationNetworkDriver());
            services.AddAndBindDriver<BodyEntity, BodyEntityFullAuthorizationNetworkDriver>(p => new BodyEntityFullAuthorizationNetworkDriver());
            services.AddAndBindDriver<BodyEntity, BodyEntityPartialAuthorizationNetworkDriver>(p => new BodyEntityPartialAuthorizationNetworkDriver());
            services.AddAndBindDriver<Ship, ShipFullAuthorizationNetworkDriver>(p => new ShipFullAuthorizationNetworkDriver());
            services.AddAndBindDriver<Ship, ShipPartialAuthorizationNetworkDriver>(p => new ShipPartialAuthorizationNetworkDriver());
            services.AddAndBindDriver<ShipPart, ShipPartFullAuthorizationNetworkDriver>(p => new ShipPartFullAuthorizationNetworkDriver());
            services.AddAndBindDriver<ShipPart, ShipPartPartialAuthorizationNetworkDriver>(p => new ShipPartPartialAuthorizationNetworkDriver());
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
