using Guppy.Network.Identity.Enums;
using Guppy.Network.Identity.Services;
using Guppy.Network.Identity;
using Guppy.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Events;
using VoidHuntersRevived.Common.Simulations.Services;
using Guppy.Attributes;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Simulations.Engines.Lockstep
{
    [AutoLoad]
    [PeerTypeFilter(PeerType.Server)]
    [SimulationTypeFilter(SimulationType.Lockstep)]
    internal class LockstepServer_UserEngine : BasicEngine
    {
        private readonly NetScope _scope;

        public LockstepServer_UserEngine(NetScope scope)
        {
            _scope = scope;
        }

        public override void Initialize(ISimulation simulation)
        {
            base.Initialize(simulation);

            _scope.Users.OnUserJoined += this.HandleUserJoined;
        }

        private void HandleUserJoined(IUserService sender, User args)
        {
            this.Simulation.Input(VhId.NewId(), new UserJoined()
            {
                UserId = args.Id,
                Claims = args.Where(x => x.Accessibility == ClaimAccessibility.Public).ToArray()
            });
        }
    }
}
