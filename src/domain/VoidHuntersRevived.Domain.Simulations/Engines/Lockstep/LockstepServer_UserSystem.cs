using Guppy.Core.Common.Attributes;
using Guppy.Core.Network.Common;
using Guppy.Core.Network.Common.Extensions;
using Guppy.Core.Network.Common.Identity.Enums;
using Guppy.Core.Network.Common.Services;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Events;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Systems.Lockstep
{
    public class LockstepServer_UserSystem(INetScope<IStrategy> scope) : StrategySystem<ILockstepStrategy>,
        IOnInitializeEngine
    {
        private readonly INetScope _scope = scope;

        [SequenceGroup<OnInitializeSequenceGroupEnum>(OnInitializeSequenceGroupEnum.Initialize)]
        public void OnInitialize(IStrategy strategy)
        {
            this._scope.Group.Users.OnUserJoined += this.HandleUserJoined;
        }

        private void HandleUserJoined(INetScopeUserService sender, IUser args)
        {
            this.Strategy.Input(VhId.NewId(), new UserJoined()
            {
                UserDto = args.ToDto(ClaimAccessibilityEnum.Public)
            });
        }
    }
}