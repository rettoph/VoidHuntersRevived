﻿using Guppy.Network.Identity.Enums;
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

namespace VoidHuntersRevived.Domain.Client
{
    public class MultiplayerGameGuppy : LocalGameGuppy
    {
        public readonly ClientPeer Client;
        public readonly NetScope NetScope;

        public MultiplayerGameGuppy(ClientPeer client, NetScope netScope, ISimulationService simulations) : base(simulations)
        {
            this.Client = client;
            this.NetScope = netScope;
        }

        public override void Initialize(IServiceProvider provider)
        {
            this.Client.Start();

            this.Client.Bind(this.NetScope, NetScopeIds.Game);

            base.Initialize(provider);

            this.Connect("192.168.0.24", 1337);
        }

        public void Connect(string host, int port)
        {
            this.Client.Connect(host, port, Claim.Create("username", "Rettoph", ClaimAccessibility.Public));
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Client.Flush();
        }
    }
}
