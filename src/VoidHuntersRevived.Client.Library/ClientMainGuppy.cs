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
        private readonly ClientPeer _client;
        private readonly IGuppyProvider _guppies;
        private readonly IScoped<GameGuppy> _game;

        public ClientMainGuppy(
            IGuppyProvider guppies,
            IGameComponentService components,
            ClientPeer client) : base(client, components)
        {
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
