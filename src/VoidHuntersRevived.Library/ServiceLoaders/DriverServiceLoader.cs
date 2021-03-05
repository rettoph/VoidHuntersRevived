using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Drivers;
using VoidHuntersRevived.Library.Drivers.Entities;
using VoidHuntersRevived.Library.Drivers.Entities.Players;
using VoidHuntersRevived.Library.Drivers.Entities.ShipParts;
using VoidHuntersRevived.Library.Drivers.Entities.ShipParts.Special;
using VoidHuntersRevived.Library.Drivers.Scenes;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Ammunitions;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Special;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.ServiceLoaders
{
    /// <summary>
    /// Service loader primarily in charge of adding base drivers
    /// into the game.
    /// </summary>
    [AutoLoad]
    internal sealed class DriverServiceLoader : IServiceLoader
    {
        public void RegisterServices(ServiceCollection services)
        {
            #region Default Filters
            services.AddDriverFilter(
                typeof(MasterNetworkAuthorizationDriver<>), 
                (d, p) => p.GetService<Settings>().Get<NetworkAuthorization>() == NetworkAuthorization.Master);

            services.AddDriverFilter(
                typeof(SlaveNetworkAuthorizationDriver<>),
                (d, p) => p.GetService<Settings>().Get<NetworkAuthorization>() == NetworkAuthorization.Slave);
            #endregion

            services.AddAndBindDriver<GameScene, GameSceneMasterNetworkAuthorizationDriver>(p => new GameSceneMasterNetworkAuthorizationDriver());
            services.AddAndBindDriver<GameScene, GameSceneSlaveNetworkAuthorizationDriver>(p => new GameSceneSlaveNetworkAuthorizationDriver());

            services.AddAndBindDriver<WorldEntity, WorldEntityMasterNetworkAuthorizationDriver>(p => new WorldEntityMasterNetworkAuthorizationDriver());
            services.AddAndBindDriver<WorldEntity, WorldEntitySlaveNetworkAuthorizationDriver>(p => new WorldEntitySlaveNetworkAuthorizationDriver());

            services.AddAndBindDriver<UserPlayer, UserPlayerMasterNetworkAuthorizationDriver>(p => new UserPlayerMasterNetworkAuthorizationDriver());

            services.AddAndBindDriver<Ship, ShipMasterNetworkAuthorizationDriver>(p => new ShipMasterNetworkAuthorizationDriver());
            services.AddAndBindDriver<Ship, ShipSlaveNetworkAuthorizationDriver>(p => new ShipSlaveNetworkAuthorizationDriver());
            services.AddAndBindDriver<Ship, ShipFighterBayMasterNetworkAuthorizationDriver>(p => new ShipFighterBayMasterNetworkAuthorizationDriver());
            services.AddAndBindDriver<Ship, ShipFighterBayDriver>(p => new ShipFighterBayDriver());

            services.AddAndBindDriver<BodyEntity, BodyEntityMasterNetworkAuthorizationDriver>(p => new BodyEntityMasterNetworkAuthorizationDriver());
            services.AddAndBindDriver<BodyEntity, BodyEntitySlaveNetworkAuthorizationDriver>(p => new BodyEntitySlaveNetworkAuthorizationDriver());

            services.AddAndBindDriver<ShipPart, ShipPartMasterNetworkAuthorizationDriver>(p => new ShipPartMasterNetworkAuthorizationDriver());
            services.AddAndBindDriver<ShipPart, ShipPartSlaveNetworkAuthorizationDriver>(p => new ShipPartSlaveNetworkAuthorizationDriver());
            
            services.AddAndBindDriver<FighterBay, FighterBayMasterAuthorizationDriver>(p => new FighterBayMasterAuthorizationDriver());

            services.AddAndBindDriver<PowerCell, PowerCellMasterAuthorizationDriver>(p => new PowerCellMasterAuthorizationDriver());
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
