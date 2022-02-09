using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network;
using Guppy.Network.Interfaces;
using Guppy.Network.Security;
using Guppy.Network.Security.Enums;
using Guppy.Network.Security.Structs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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
        public override Peer Peer => this.Client;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _graphics);

            this.Client = provider.GetService<ClientPeer>();
        }

        protected override void PostInitialize(ServiceProvider provider)
        {
            base.PostInitialize(provider);

            this.Client.TryStart();
            this.Client.TryConnect("localhost", 1337, new[] { new Claim("username", "Rettoph", ClaimType.Public) });
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            _graphics.Clear(Color.Black);

            base.Draw(gameTime);

            this.Scenes.TryDraw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Scenes.TryUpdate(gameTime);
        }
        #endregion
    }
}
