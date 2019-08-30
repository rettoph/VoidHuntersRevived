using GalacticFighters.Library;
using Guppy.Network.Peers;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Client.Library
{
    public class ClientGalacticFightersGame : GalacticFightersGame
    {
        private ClientPeer _client;

        public ClientGalacticFightersGame(ClientPeer client) : base(client)
        {
            _client = client;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _client.TryConnect("127.0.0.1", 1337);
        }
    }
}
