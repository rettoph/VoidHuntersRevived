using Guppy.Core.Common.Attributes;
using Guppy.Core.Network;
using Guppy.Core.Network.Attributes;
using Guppy.Core.Network.Enums;
using Guppy.Core.Network.Extensions.Identity;
using Guppy.Core.Network.Identity;
using Guppy.Core.Network.Identity.Enums;
using Guppy.Core.Network.Identity.Services;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Events;

namespace VoidHuntersRevived.Domain.Simulations.Engines.Lockstep
{
    [AutoLoad]
    [PeerFilter(PeerType.Server)]
    [SimulationFilter(SimulationType.Lockstep)]
    internal class LockstepServer_UserEngine : BasicEngine
    {
        private readonly INetGroup _scope;

        public LockstepServer_UserEngine(INetGroup scope)
        {
            _scope = scope;
        }

        public override void Initialize(ISimulation simulation)
        {
            base.Initialize(simulation);

            _scope.Users.OnUserJoined += this.HandleUserJoined;
        }

        private void HandleUserJoined(INetScopeUserService sender, User args)
        {
            this.Simulation.Input(VhId.NewId(), new UserJoined()
            {
                UserDto = args.ToDto(ClaimAccessibility.Public)
            });
        }
    }
}
