using GalacticFighters.Library;
using Guppy.Network.Peers;
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
        }

        protected override void Initialize()
        {
            base.Initialize();

            _server.Messages.TryAdd(NetIncomingMessageType.StatusChanged, this.HandleStatusChanged);
            _server.Messages.TryAdd(NetIncomingMessageType.ConnectionApproval, this.HandleConnectionApproval);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        private void HandleStatusChanged(object sender, NetIncomingMessage arg)
        {
            this.logger.LogDebug($"Status => {arg.SenderConnection.Status}");
        }

        private void HandleConnectionApproval(object sender, NetIncomingMessage arg)
        {
            throw new NotImplementedException();
        }
    }
}
