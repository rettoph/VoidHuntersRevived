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
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
