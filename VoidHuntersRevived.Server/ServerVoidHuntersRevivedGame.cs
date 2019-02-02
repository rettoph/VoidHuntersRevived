using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using VoidHuntersRevived.Core.Loaders;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Networking.Interfaces;
using VoidHuntersRevived.Networking.Peers;
using VoidHuntersRevived.Server.Scenes;
using VoidHuntersRevived.Core.Extensions;
using VoidHuntersRevived.Server.Entities.Drivers;

namespace VoidHuntersRevived.Server
{
    /// <summary>
    /// The server specific implementation of the 
    /// VoidHUntersRevivedGame class
    /// </summary>
    public class ServerVoidHuntersRevivedGame : VoidHuntersRevivedGame
    {
        #region Private Fields
        private ServerPeer _server;
        #endregion

        #region Constructors
        public ServerVoidHuntersRevivedGame(
            ILogger logger,
            GraphicsDeviceManager graphics = null,
            ContentManager content = null,
            GameWindow window = null,
            IServiceCollection services = null) : base(logger, graphics, content, window, services)
        {
        }
        #endregion

        #region Initialization Methods
        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            // Add a new peer to the service collection
            services.AddSingleton<IPeer>(new ServerPeer("vhr", 1337, this, this.Logger));
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Save the current servers peer
            _server = this.Provider.GetService<IPeer>() as ServerPeer;

            // Register server specific entities
            var entityLoader = this.Provider.GetLoader<EntityLoader>();
            entityLoader.Register<ServerShipPartDriver>("entity:driver:ship_part");
            entityLoader.Register<ServerUserPlayerDriver>("entity:driver:user_player");
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Create a new scene instance
            var scene = this.Scenes.Create<ServerMainGameScene>();

            // Add peer event handlers
            this.Peer.Users.OnAdded += this.HandleUserAdd;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles users connecting to the server peer successfully.
        /// Auto add the user to the main game scene group id
        /// Currently, that value is hardcoded to 420
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleUserAdd(object sender, IUser e)
        {
            this.Peer.Groups.GetById(420).Users.Add(e);
        }
        #endregion
    }
}