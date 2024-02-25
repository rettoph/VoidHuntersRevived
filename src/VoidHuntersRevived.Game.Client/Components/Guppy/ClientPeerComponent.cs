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
        private readonly IClientPeer _client;
        private readonly INetScope _scope;

        public ClientPeerComponent(IClientPeer client, INetScope scope)
        {
            _client = client;
            _scope = scope;
        }

        public void Initialize(IGuppy guppy)
        {
            _client.Start();
            _scope.AttachPeer(_client, NetScopeIds.Game);
        }

        public void Update(GameTime gameTime)
        {
            _client.Flush();
        }
    }
}
