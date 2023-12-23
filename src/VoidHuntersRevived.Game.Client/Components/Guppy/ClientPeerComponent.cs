using Guppy;
using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Enums;
using Guppy.Game.Common;
using Guppy.Game.Common.Enums;
using Guppy.Network;
using Guppy.Network.Peers;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Constants;

namespace VoidHuntersRevived.Game.Client.Components.Guppy
{
    [AutoLoad]
    [GuppyFilter<MultiplayerGameGuppy>]
    [Sequence<InitializeSequence>(InitializeSequence.Setup)]
    [Sequence<UpdateSequence>(UpdateSequence.PostUpdate)]
    internal class ClientPeerComponent : IGuppyComponent, IGuppyUpdateable
    {
        private readonly ClientPeer _client;
        private readonly NetScope _scope;

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
