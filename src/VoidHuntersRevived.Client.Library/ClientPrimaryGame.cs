using Guppy.DependencyInjection;
using Guppy.Network.Interfaces;
using Guppy.Network.Peers;
using Guppy.Network.Security;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library;

namespace VoidHuntersRevived.Client.Library
{
    public class ClientPrimaryGame : PrimaryGame
    {
        #region Private Fields
        private GraphicsDevice _graphics;
        #endregion

        #region Public Properties
        public ClientPeer Client { get; private set; }
        public override IPeer Peer => this.Client;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _graphics);

            this.Client = provider.GetService<ClientPeer>();
        }

        protected override void PostInitialize(GuppyServiceProvider provider)
        {
            base.PostInitialize(provider);

            this.Client.TryConnect("localhost", 1337, new Claim("username", "Rettoph"));
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            _graphics.Clear(Color.Black);

            base.Draw(gameTime);
        }
        #endregion
    }
}
