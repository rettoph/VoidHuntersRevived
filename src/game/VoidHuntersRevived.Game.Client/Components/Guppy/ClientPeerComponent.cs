using Guppy.Core.Common.Attributes;
using Guppy.Core.Network.Common.Peers;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common;
using Guppy.Game.Common.Attributes;
using Guppy.Game.Common.Components;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Game.Client.Components.Guppy
{
    [AutoLoad]
    [SceneFilter<MultiplayerGameScene>]
    [Sequence<InitializeSequence>(InitializeSequence.Setup)]
    [Sequence<UpdateSequence>(UpdateSequence.PostUpdate)]
    internal class ClientPeerComponent : SceneComponent, IGuppyUpdateable
    {
        private readonly IClientPeer _client;

        public ClientPeerComponent(IClientPeer client)
        {
            _client = client;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _client.Start();
        }

        public void Update(GameTime gameTime)
        {
            _client.Flush();
        }
    }
}
