using Guppy.Network.Identity.Enums;
using Guppy.Network.Peers;
using Guppy.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Constants;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations;
using Guppy.Network.Identity.Claims;
using Microsoft.Xna.Framework;
using Autofac;

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

            this.Connect("24.116.126.35", 1337);
        }

        public void Connect(string host, int port)
        {
            this.Client.Connect(host, port, Claim.Create("username", "Rettoph", ClaimAccessibility.Public));
        }
    }
}
