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

namespace VoidHuntersRevived.Game.Client
{
    public class LocalGameGuppy : GameGuppy
    {
        public LocalGameGuppy(ISimulationService simulations) : base(simulations)
        {
        }

        public override void Initialize(IServiceProvider provider)
        {
            this.Simulations.Configure(SimulationType.Lockstep | SimulationType.Predictive);

            base.Initialize(provider);
        }
    }
}
