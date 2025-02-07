using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Guppy.Core.Common.Systems;
using Guppy.Core.Network.Common;
using Guppy.Core.Network.Common.Extensions;
using Guppy.Core.Network.Common.Identity.Enums;
using Guppy.Core.Network.Common.Services;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Events;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Systems.Lockstep
{
    public class LockstepServer_UserSystem(
        ILockstepStrategy strategy,
        INetScope<IStrategy> scope
    ) : ISceneSystem,
        IInitializeSystem
    {
        private readonly INetScope _scope = scope;
        private readonly ILockstepStrategy _strategy = strategy;

        [SequenceGroup<InitializeSequenceGroupEnum>(InitializeSequenceGroupEnum.Initialize)]
        public void Initialize()
        {
            this._scope.Group.Users.OnUserJoined += this.HandleUserJoined;
        }

        private void HandleUserJoined(INetScopeUserService sender, IUser args)
        {
            this._strategy.Input(VhId.NewId(), new UserJoined()
            {
                UserDto = args.ToDto(ClaimAccessibilityEnum.Public)
            });
        }
    }
}