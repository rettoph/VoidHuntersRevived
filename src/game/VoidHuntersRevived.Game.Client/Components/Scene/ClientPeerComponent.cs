using Guppy.Core.Common.Attributes;
using Guppy.Core.Network.Common.Peers;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common.Components;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Game.Client.Components.Scene
{
    internal class ClientPeerComponent(IClientPeer client) : ISceneComponent<MultiplayerGameScene>, IUpdatableComponent
    {
        private readonly IClientPeer _client = client;

        [SequenceGroup<InitializeComponentSequenceGroupEnum>(InitializeComponentSequenceGroupEnum.Setup)]
        public void Initialize(MultiplayerGameScene scene) => this._client.Start();

        [SequenceGroup<UpdateComponentSequenceGroupEnum>(UpdateComponentSequenceGroupEnum.PostUpdate)]
        public void Update(GameTime gameTime) => this._client.Flush();
    }
}