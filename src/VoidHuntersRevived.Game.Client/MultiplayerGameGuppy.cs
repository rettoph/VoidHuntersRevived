using Autofac;
using Guppy.Network.Identity.Claims;
using Guppy.Network.Identity.Enums;
using Guppy.Network.Peers;

namespace VoidHuntersRevived.Game.Client
{
    public class MultiplayerGameGuppy : LocalGameGuppy
    {
        public readonly ClientPeer Client;

        public MultiplayerGameGuppy(ClientPeer client)
        {
            this.Client = client;
        }

        public override void Initialize(ILifetimeScope scope)
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
