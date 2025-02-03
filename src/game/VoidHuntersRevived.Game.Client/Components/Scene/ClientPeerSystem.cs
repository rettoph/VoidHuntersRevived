using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Guppy.Core.Network.Common.Peers;
using Guppy.Game.Common.Systems;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Game.Client.Components.Scene
{
    public class ClientPeerSystem(IClientPeer client) : ISceneSystem<MultiplayerGameScene>, IUpdatableSystem
    {
        private readonly IClientPeer _client = client;

        [SequenceGroup<InitializeSystemSequenceGroupEnum>(InitializeSystemSequenceGroupEnum.Setup)]
        public void Initialize(MultiplayerGameScene scene)
        {
            this._client.Start();
        }

        [SequenceGroup<UpdateComponentSequenceGroupEnum>(UpdateComponentSequenceGroupEnum.PostUpdate)]
        public void Update(GameTime gameTime)
        {
            this._client.Flush();
        }
    }
}