using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Guppy;
using Guppy.Extensions.Collection;
using Guppy.Network.Peers;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Client.Library
{
    /// <summary>
    /// The client specific version of the BaseGame class
    /// </summary>
    public class ClientGame : BaseGame
    {
        #region Private Fields
        private ClientPeer _client;
        private Scene _scene;
        #endregion

        #region Constructor
        public ClientGame(ClientPeer client) : base(client)
        {
            _client = client;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            // _client.TryConnect("localhost", 1337, _client.Users.Create("Tony"));

            // Create a main menu scene
            this.scenes.Create<MainMenuScene>();
            // this.scenes.Create<ClientWorldScene>(s =>
            // {
            //     s.Group = _client.Groups.GetOrCreateById(Guid.Empty);
            // });
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.scenes.TryDrawAll(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.scenes.TryUpdateAll(gameTime);
        }
        #endregion
    }
}
