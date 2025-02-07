using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Guppy.Core.Common.Systems;
using Guppy.Core.Network.Common.Peers;
using Guppy.Game.Common.Enums;
using Guppy.Game.Common.Systems;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Game.Client.Components.Scene
{
    public class ClientPeerSystem(IClientPeer client) : ISceneSystem, IInitializeSystem, IUpdateSystem
    {
        private readonly IClientPeer _client = client;

        [SequenceGroup<InitializeSequenceGroupEnum>(InitializeSequenceGroupEnum.Setup)]
        public void Initialize()
        {
            this._client.Start();
        }

        [SequenceGroup<UpdateSequenceGroupEnum>(UpdateSequenceGroupEnum.PostUpdate)]
        public void Update(GameTime gameTime)
        {
            this._client.Flush();
        }
    }
}