using Guppy.Common;
using Guppy.MonoGame;
using Guppy.MonoGame.Providers;
using Guppy.MonoGame.Services;
using Guppy.Network.Peers;
using Guppy.Providers;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoidHuntersRevived.Library;

namespace VoidHuntersRevived.Client.Library
{
    internal sealed class ClientMainGuppy : MainGuppy
    {
        private ClientPeer _client;
        private IGuppyProvider _guppies;
        private IScoped<GameGuppy> _game;

        public ClientMainGuppy(
            IGuppyProvider guppies,
            ClientPeer client,
            World world) : base(client, world)
        {
            Thread.Sleep(2000);

            _guppies = guppies;
            _client = client;

            _client.Start();

            _client.Connect("localhost", 1337);

            _game = _guppies.Create<ClientGameGuppy>();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _game.Instance.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _game.Instance.Draw(gameTime);
        }
    }
}
