using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Network.Extensions.Identity;
using Guppy.Network.Identity;
using Guppy.Network.Identity.Enums;
using Guppy.Network.Identity.Services;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Events;

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
