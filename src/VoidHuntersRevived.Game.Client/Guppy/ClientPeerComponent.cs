using Guppy;
using Guppy.Attributes;
using Guppy.MonoGame;
using Guppy.Network.Peers;
using Guppy.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Constants;
using Guppy.MonoGame.Common;
using Microsoft.Xna.Framework;
using Guppy.Common.Attributes;
using Guppy.MonoGame.Common.Enums;
using Guppy.Enums;

namespace VoidHuntersRevived.Game.Client.Guppy
{
    [AutoLoad]
    [GuppyFilter<MultiplayerGameGuppy>]
    [Sequence<InitializeSequence>(InitializeSequence.PreInitialize)]
    [Sequence<UpdateSequence>(UpdateSequence.PostUpdate)]
    internal class ClientPeerComponent : IGuppyComponent, IGuppyUpdateable
    {
        public readonly ClientPeer _client;
        public readonly NetScope _scope;

        public ClientPeerComponent(ClientPeer client, NetScope scope)
        {
            _client = client;
            _scope = scope;
        }

        public void Initialize(IGuppy guppy)
        {
            _client.Start();
            _client.Bind(_scope, NetScopeIds.Game);
        }

        public void Update(GameTime gameTime)
        {
            _client.Flush();
        }
    }
}
