using VoidHuntersRevived.Library;
using VoidHuntersRevived.Server.Scenes;
using Guppy.Network.Peers;
using Guppy.Network.Security;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Server
{
    public class ServerVoidHuntersRevivedGame : VoidHuntersRevivedGame
    {
        private ServerPeer _server;

        public ServerVoidHuntersRevivedGame(ServerPeer server) : base(server)
        {
            _server = server;
            _server.Users.Events.TryAdd<User>("added", this.HandleUserJoined);
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.scenes.Create<ServerWorldScene>(s =>
            {
                s.Group = _server.Groups.GetOrCreateById(Guid.Empty);
            }).TryStartAsync();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        #region Event Handlers
        private void HandleUserJoined(object sender, User arg)
        {
            _server.Groups.GetOrCreateById(Guid.Empty).Users.Add(arg);
        }
        #endregion
    }
}
