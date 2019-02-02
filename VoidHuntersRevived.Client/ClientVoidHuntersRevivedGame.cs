using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using VoidHuntersRevived.Client.Scenes;
using VoidHuntersRevived.Core.Loaders;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Networking.Interfaces;
using VoidHuntersRevived.Networking.Peers;
using VoidHuntersRevived.Core.Extensions;
using VoidHuntersRevived.Client.Entities.Drivers;
using VoidHuntersRevived.Client.Entities;
using VoidHuntersRevived.Core.Providers;
using Microsoft.Xna.Framework.Graphics;

namespace VoidHuntersRevived.Client
{
    /// <summary>
    /// The client specific implementation of the 
    /// VoidHUntersRevivedGame class
    /// </summary>
    public class ClientVoidHuntersRevivedGame : VoidHuntersRevivedGame
    {
        #region Private Fields
        private ClientPeer _client;
        #endregion

        #region Constructors
        public ClientVoidHuntersRevivedGame(
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
            services.AddSingleton<IPeer>(new ClientPeer("vhr", this, this.Logger));
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Save the current clients peer
            _client = this.Provider.GetService<IPeer>() as ClientPeer;

            // Register client specific assets
            var contentLoader = this.Provider.GetLoader<ContentLoader>();
            contentLoader.Register<SpriteFont>("font:debug", "Fonts/debug");
            contentLoader.Register<Texture2D>("texture:connection_node:male", "Sprites/male-connection");
            contentLoader.Register<Texture2D>("texture:connection_node:female", "Sprites/female-connection");

            // Register client specific entities
            var entityLoader = this.Provider.GetLoader<EntityLoader>();
            entityLoader.Register<Camera>("entity:camera");
            entityLoader.Register<ClientShipPartDriver>("entity:driver:ship_part");
            entityLoader.Register<ClientUserPlayerDriver>("entity:driver:user_player");
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Create a new scene instance
            var scene = this.Scenes.Create<ClientMainGameScene>();
        }

        protected override void PostInitialize()
        {
            base.PostInitialize();

            // Auto login to localhost
            var hail = _client.CreateMessage("network:user:connection-request");
            hail.Write("tony");
            _client.Connect("localhost", 1337, hail);
        }
        #endregion
    }
}
