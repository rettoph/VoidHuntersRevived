using GalacticFighters.Client.Library.Scenes;
using GalacticFighters.Library;
using Guppy;
using Guppy.Network.Peers;
using Guppy.Network.Security;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Client.Library
{
    public class ClientGalacticFightersGame : GalacticFightersGame
    {
        private ClientPeer _client;
        private Scene _scene;

        public ClientGalacticFightersGame(ClientPeer client) : base(client)
        {
            _client = client;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _client.MessagesTypes.TryAdd(NetIncomingMessageType.StatusChanged, this.HandleStatusChanged);

            _client.TryConnect("127.0.0.1", 1337, _client.Users.Create("Rettoph"));

            _scene = this.scenes.Create<ClientGalacticFightersWorldScene>(s =>
            {
                s.Group = _client.Groups.GetOrCreateById(Guid.Empty);
            });
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _scene.TryUpdate(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _scene.TryDraw(gameTime);
        }

        private void HandleStatusChanged(object sender, NetIncomingMessage arg)
        {
            this.logger.LogDebug($"Status => {arg.SenderConnection.Status}");
        }
    }
}
