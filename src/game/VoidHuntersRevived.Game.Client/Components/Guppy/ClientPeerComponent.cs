using Guppy.Core.Common.Attributes;
using Guppy.Core.Network.Common.Peers;
using Guppy.Engine.Common;
using Guppy.Engine.Common.Components;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Game.Client.Components.Guppy
{
    [AutoLoad]
    [GuppyFilter<MultiplayerGameGuppy>]
    [Sequence<InitializeSequence>(InitializeSequence.Setup)]
    [Sequence<UpdateSequence>(UpdateSequence.PostUpdate)]
    internal class ClientPeerComponent : IGuppyComponent, IGuppyUpdateable
    {
        private readonly IClientPeer _client;

        public ClientPeerComponent(IClientPeer client)
        {
            _client = client;
        }

        public void Initialize(IGuppy guppy)
        {
            _client.Start();
        }

        public void Update(GameTime gameTime)
        {
            _client.Flush();
        }
    }
}
