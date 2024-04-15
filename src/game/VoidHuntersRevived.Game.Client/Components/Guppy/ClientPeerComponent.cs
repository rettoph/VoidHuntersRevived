using Guppy.Core.Common.Attributes;
using Guppy.Core.Network;
using Guppy.Core.Network.Peers;
using Guppy.Engine.Common;
using Guppy.Engine.Common.Components;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Common.Constants;

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
            _client.Groups.GetById(NetScopeIds.Game).Attach(_scope);
        }

        public void Update(GameTime gameTime)
        {
            _client.Flush();
        }
    }
}
