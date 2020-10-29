using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Drivers;
using VoidHuntersRevived.Library.Drivers.Entities;
using VoidHuntersRevived.Library.Drivers.Scenes;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
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
        public void ConfigureServices(ServiceCollection services)
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
        }

        public void ConfigureProvider(ServiceProvider provider)
        {
            // throw new NotImplementedException();
        }
    }
}
