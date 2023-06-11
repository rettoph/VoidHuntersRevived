using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Network.Identity;
using Guppy.Network.Identity.Enums;
using Guppy.Network.Identity.Services;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.ECS;
using VoidHuntersRevived.Common.ECS.Systems;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Domain.Simulations.Events;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    [AutoLoad]
    [PeerTypeFilter(PeerType.Server)]
    [SimulationTypeFilter(SimulationType.Lockstep)]
    internal sealed class LockstepServer_UserSystem : BasicSystem,
        IEventSubscriber<UserJoined>
    {
        private readonly NetScope _scope;
        private readonly ISimulationService _simulations;

        public LockstepServer_UserSystem(NetScope scope, ISimulationService simulations)
        {
            _scope = scope;
            _simulations = simulations;
        }

        public override void Initialize(IWorld world)
        {
            base.Initialize(world);

            _scope.Users.OnUserJoined += this.HandleUserJoined;
        }

        public void Process(in EventId id, UserJoined data)
        {
            
        }

        private void HandleUserJoined(IUserService sender, User args)
        {
            _simulations.Enqueue(new UserJoined()
            {
                UserId = args.Id,
                Claims = args.Where(x => x.Accessibility == ClaimAccessibility.Public).ToArray()
            });
        }
    }
}
