using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Network.Extensions.Identity;
using Guppy.Network.Identity;
using Guppy.Network.Identity.Enums;
using Guppy.Network.Identity.Services;
using Guppy.Network.Messages;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Services;
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
            _simulations.PublishEvent(
                source: SimulationType.Predictive, 
                data: new PlayerAction()
                {
                   UserAction = args.CreateAction(UserAction.Actions.UserJoined, ClaimAccessibility.Public)
                });
        }
    }
}
