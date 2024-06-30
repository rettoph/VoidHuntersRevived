using Guppy.Core.Common.Attributes;
using Guppy.Core.Network.Common;
using Guppy.Core.Network.Common.Attributes;
using Guppy.Core.Network.Common.Enums;
using Guppy.Core.Network.Common.Extensions;
using Guppy.Core.Network.Common.Identity.Enums;
using Guppy.Core.Network.Common.Services;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Events;

namespace VoidHuntersRevived.Domain.Simulations.Engines.Lockstep
{
    [AutoLoad]
    [PeerFilter(PeerType.Server)]
    [StrategyFilter(StrategyTypeEnum.Lockstep)]
    internal class LockstepServer_UserEngine : BasicEngine
    {
        private readonly INetScope _scope;

        public LockstepServer_UserEngine(INetScope<IStrategy> scope)
        {
            _scope = scope;
        }

        public override void Initialize(IStrategy strategy)
        {
            base.Initialize(strategy);

            _scope.Group.Users.OnUserJoined += this.HandleUserJoined;
        }

        private void HandleUserJoined(INetScopeUserService sender, IUser args)
        {
            this.Simulation.Input(VhId.NewId(), new UserJoined()
            {
                UserDto = args.ToDto(ClaimAccessibility.Public)
            });
        }
    }
}
