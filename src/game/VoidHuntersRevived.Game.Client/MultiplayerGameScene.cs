using Autofac;
using Guppy.Core.Network.Common.Claims;
using Guppy.Core.Network.Common.Identity.Enums;
using Guppy.Core.Network.Common.Peers;

namespace VoidHuntersRevived.Game.Client
{
    public class MultiplayerGameScene(IClientPeer client) : LocalGameScene
    {
        public readonly IClientPeer Client = client;

        protected override void Initialize(ILifetimeScope scope)
        {
            base.Initialize(scope);

            this.Connect("localhost", 1337);
        }

        public void Connect(string host, int port)
        {
            this.Client.Connect(host, port, Claim.Create("username", "Rettoph", ClaimAccessibility.Public));
        }
    }
}
