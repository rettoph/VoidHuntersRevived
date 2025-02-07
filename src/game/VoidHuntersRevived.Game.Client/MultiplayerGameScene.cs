using Guppy.Core.Common;
using Guppy.Core.Network.Common.Claims;
using Guppy.Core.Network.Common.Identity.Enums;
using Guppy.Core.Network.Common.Peers;

namespace VoidHuntersRevived.Game.Client
{
    public class MultiplayerGameScene(IClientPeer client, IGuppyScope scope) : LocalGameScene(scope)
    {
        public readonly IClientPeer Client = client;

        protected override void Initialize()
        {
            base.Initialize();

            this.Connect("localhost", 1337);
        }

        public void Connect(string host, int port)
        {
            this.Client.Connect(host, port, Claim.Create("username", "Rettoph", ClaimAccessibilityEnum.Public));
        }
    }
}