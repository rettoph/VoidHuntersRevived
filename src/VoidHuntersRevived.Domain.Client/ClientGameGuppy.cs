using Guppy.Network;
using Guppy.Network.Identity.Claims;
using Guppy.Network.Identity.Enums;
using Guppy.Network.Peers;
using Guppy.Resources.Providers;
using System;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Constants;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Domain.Client
{
    public class ClientGameGuppy : GameGuppy
    {
        public readonly ClientPeer Client;

        public ClientGameGuppy(ClientPeer client, NetScope netScope, ISimulationService simulations) : base(client, netScope, simulations)
        {
            this.Client = client;
        }

        public override void Initialize(IServiceProvider provider)
        {
            this.Client.Start();

            this.Peer.Bind(this.NetScope, NetScopeIds.Game);

            this.Simulations.Initialize(SimulationType.Lockstep | SimulationType.Predictive);

            base.Initialize(provider);

            this.Connect("localhost", 1337);
        }

        public void Connect(string host, int port)
        {
            this.Client.Connect(host, port, Claim.Create("username", "Rettoph", ClaimAccessibility.Public));
        }
    }
}
