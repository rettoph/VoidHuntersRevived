using Guppy.Core.Common.Attributes;
using Guppy.Core.Network.Common.Peers;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common;
using Guppy.Game.Common.Attributes;
using Guppy.Game.Common.Components;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Game.Client.Components.Scene
{
    [SceneFilter<MultiplayerGameScene>]
    internal class ClientPeerComponent(IClientPeer client) : ISceneComponent<IScene>, IUpdatableComponent
    {
        private readonly IClientPeer _client = client;

        [SequenceGroup<InitializeComponentSequenceGroup>(InitializeComponentSequenceGroup.Setup)]
        public void Initialize(IScene scene)
        {
            _client.Start();
        }

        [SequenceGroup<UpdateComponentSequenceGroup>(UpdateComponentSequenceGroup.PostUpdate)]
        public void Update(GameTime gameTime)
        {
            _client.Flush();
        }
    }
}
