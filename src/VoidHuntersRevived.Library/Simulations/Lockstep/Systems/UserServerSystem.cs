using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Network.Identity;
using Guppy.Network.Identity.Services;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Attributes;
using VoidHuntersRevived.Common.Services;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Library.Simulations.Events;

namespace VoidHuntersRevived.Library.Simulations.Lockstep.Systems
{
    [PeerTypeFilter(PeerType.Server)]
    [SimulationTypeFilter(SimulationType.Lockstep)]
    internal sealed class UserServerSystem : BasicSystem
    {
        private readonly NetScope _scope;
        private readonly ISimulationService _simulations;

        public UserServerSystem(NetScope scope, ISimulationService simulations)
        {
            _scope = scope;
            _simulations = simulations;
        }

        public override void Initialize(World world)
        {
            base.Initialize(world);

            _scope.Users.OnUserJoined += this.HandleUserJoined;
        }

        private void HandleUserJoined(IUserService sender, User args)
        {
            _simulations[SimulationType.Lockstep].PublishEvent(PeerType.Client, new UserJoined());
        }
    }
}
