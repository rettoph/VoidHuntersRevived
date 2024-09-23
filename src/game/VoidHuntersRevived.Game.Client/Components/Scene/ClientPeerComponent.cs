using Guppy.Core.Common.Attributes;
using Guppy.Core.Network.Common.Peers;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common.Attributes;
using Guppy.Game.Common.Components;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Game.Client.Components.Scene
{
    [AutoLoad]
    [SceneFilter<MultiplayerGameScene>]
    [SequenceGroup<InitializeSequence>(InitializeSequence.Setup)]
    internal class ClientPeerComponent : SceneComponent, IUpdatableComponent
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

        [SequenceGroup<UpdateComponentSequenceGroup>(UpdateComponentSequenceGroup.PostUpdate)]
        public void Update(GameTime gameTime)
        {
            _client.Flush();
        }
    }
}
