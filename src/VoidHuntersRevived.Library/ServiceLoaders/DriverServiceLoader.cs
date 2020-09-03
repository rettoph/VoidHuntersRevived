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
using VoidHuntersRevived.Library.Drivers.Entities.ShipParts;
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
            services.AddAndBindDriver<GameScene, GameSceneMinimumAuthorityNetworkDriver>(p => new GameSceneMinimumAuthorityNetworkDriver());

            services.AddAndBindDriver<Player, PlayerFullAuthorizationNetworkDriver>(p => new PlayerFullAuthorizationNetworkDriver());
            services.AddAndBindDriver<Player, PlayerMinimumAuthorizationNetworkDriver>(p => new PlayerMinimumAuthorizationNetworkDriver());
            services.AddAndBindDriver<UserPlayer, UserPlayerFullAuthorizationNetworkDriver>(p => new UserPlayerFullAuthorizationNetworkDriver());
            services.AddAndBindDriver<UserPlayer, UserPlayerMinimumAuthorizationNetworkDriver>(p => new UserPlayerMinimumAuthorizationNetworkDriver());
            services.AddAndBindDriver<WorldEntity, WorldEntityNetworkDriver>(p => new WorldEntityNetworkDriver());
            services.AddAndBindDriver<BodyEntity, BodyEntityNetworkDriver>(p => new BodyEntityNetworkDriver());
            services.AddAndBindDriver<Ship, ShipFullAuthorizationNetworkDriver>(p => new ShipFullAuthorizationNetworkDriver());
            services.AddAndBindDriver<Ship, ShipMinimumAuthorizationNetworkDriver>(p => new ShipMinimumAuthorizationNetworkDriver());
            services.AddAndBindDriver<ShipPart, ShipPartNetworkDriver>(p => new ShipPartNetworkDriver());
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
