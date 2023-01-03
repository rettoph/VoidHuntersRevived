using Guppy.Network;
using Guppy.Network.Identity.Claims;
using Guppy.Network.Identity.Enums;
using Guppy.Network.Peers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Services;

namespace VoidHuntersRevived.Library.Client
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

            this.Peer.Bind(this.NetScope, 0);

            this.Simulations.Initialize(SimulationType.Lockstep | SimulationType.Predictive);

            base.Initialize(provider);

            this.Client.Connect("localhost", 1337, Claim.Create("username", "Rettoph", ClaimAccessibility.Public));
        }
    }
}
