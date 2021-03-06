﻿using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Drivers;
using VoidHuntersRevived.Library.Drivers.Entities;
using VoidHuntersRevived.Library.Drivers.Entities.Players;
using VoidHuntersRevived.Library.Drivers.Entities.ShipActionDrivers;
using VoidHuntersRevived.Library.Drivers.Entities.ShipParts;
using VoidHuntersRevived.Library.Drivers.Entities.ShipParts.Special;
using VoidHuntersRevived.Library.Drivers.Entities.ShipParts.SpellParts;
using VoidHuntersRevived.Library.Drivers.Scenes;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Special;
using VoidHuntersRevived.Library.Entities.ShipParts.SpellParts;
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

            services.AddAndBindDriver<Explosion, ExplosionMasterNetworkAuthorizationDriver>(p => new ExplosionMasterNetworkAuthorizationDriver());
            services.AddAndBindDriver<Explosion, ExplosionSlaveNetworkAuthorizationDriver>(p => new ExplosionSlaveNetworkAuthorizationDriver());

            services.AddAndBindDriver<UserPlayer, UserPlayerMasterNetworkAuthorizationDriver>(p => new UserPlayerMasterNetworkAuthorizationDriver());

            services.AddAndBindDriver<SpellCaster, SpellCasterMasterNetworkAuthorizationDriver>(p => new SpellCasterMasterNetworkAuthorizationDriver());
            services.AddAndBindDriver<SpellCaster, SpellCasterSlaveNetworkAuthorizationDriver>(p => new SpellCasterSlaveNetworkAuthorizationDriver());

            services.AddAndBindDriver<Ship, ShipMasterNetworkAuthorizationDriver>(p => new ShipMasterNetworkAuthorizationDriver());
            services.AddAndBindDriver<Ship, ShipSlaveNetworkAuthorizationDriver>(p => new ShipSlaveNetworkAuthorizationDriver());
            services.AddAndBindDriver<Ship, ShipActionSlaveNetworkAuthorizationDriver>(p => new ShipActionSlaveNetworkAuthorizationDriver());
            services.AddAndBindDriver<Ship, ShipTryLaunchDronesActionDriver>(p => new ShipTryLaunchDronesActionDriver());
            services.AddAndBindDriver<Ship, ShipTryToggleEnergyShieldsActionDriver>(p => new ShipTryToggleEnergyShieldsActionDriver());

            services.AddAndBindDriver<BodyEntity, BodyEntityMasterNetworkAuthorizationDriver>(p => new BodyEntityMasterNetworkAuthorizationDriver());
            services.AddAndBindDriver<BodyEntity, BodyEntitySlaveNetworkAuthorizationDriver>(p => new BodyEntitySlaveNetworkAuthorizationDriver());

            services.AddAndBindDriver<ShipPart, ShipPartMasterNetworkAuthorizationDriver>(p => new ShipPartMasterNetworkAuthorizationDriver());
            services.AddAndBindDriver<ShipPart, ShipPartSlaveNetworkAuthorizationDriver>(p => new ShipPartSlaveNetworkAuthorizationDriver());

            services.AddAndBindDriver<SpellPart, SpellPartMasterAuthorizationNetworkDriver>(p => new SpellPartMasterAuthorizationNetworkDriver());
            services.AddAndBindDriver<SpellPart, SpellPartSlaveAuthorizationNetworkDriver>(p => new SpellPartSlaveAuthorizationNetworkDriver());

            services.AddAndBindDriver<PowerCell, PowerCellMasterAuthorizationDriver>(p => new PowerCellMasterAuthorizationDriver());
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
