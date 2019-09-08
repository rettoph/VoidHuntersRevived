using GalacticFighters.Library;
using GalacticFighters.Server.Scenes;
using Guppy.Network.Peers;
using Guppy.Network.Security;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Server
{
    public class ServerGalacticFightersGame : GalacticFightersGame
    {
        private ServerPeer _server;

        public ServerGalacticFightersGame(ServerPeer server) : base(server)
        {
            _server = server;
            _server.Users.Events.TryAdd<User>("added", this.HandleUserJoined);
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.scenes.Create<ServerGalacticFightersWorldScene>(s =>
            {
                s.Group = _server.Groups.GetOrCreateById(Guid.Empty);
            });
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
