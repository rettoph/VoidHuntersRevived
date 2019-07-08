using Guppy;
using Guppy.Network.Peers;
using Guppy.Network.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Server.Scenes;

namespace VoidHuntersRevived.Server
{
    class VoidHuntersServerGame : VoidHuntersGame
    {
        protected ServerPeer server;

        public VoidHuntersServerGame(ServerPeer server, IServiceProvider provider) : base(provider)
        {
            this.server = server;
        }

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.SetScene(this.CreateScene<VoidHuntersServerWorldScene>());
            this.server.OnUserConnected += this.HandleUserConnected;
        }
        #endregion

        #region Frame Methods
        protected override void update(GameTime gameTime)
        {
            this.server.Update();

            base.update(gameTime);
        }
        #endregion

        private void HandleUserConnected(object sender, User e)
        {
            this.server.Groups.GetOrCreateById(Guid.Empty).Users.Add(e);
        }
    }
}
