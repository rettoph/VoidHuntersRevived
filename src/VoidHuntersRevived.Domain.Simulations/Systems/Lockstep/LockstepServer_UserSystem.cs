using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Network.Extensions.Identity;
using Guppy.Network.Identity;
using Guppy.Network.Identity.Enums;
using Guppy.Network.Identity.Services;
using Guppy.Network.Messages;
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Enums;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Domain.Simulations.Events;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Messages;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    [PeerTypeFilter(PeerType.Server)]
    [SimulationTypeFilter(SimulationType.Lockstep)]
    internal sealed class LockstepServer_UserSystem : BasicSystem,
        ISimulationEventListener<UserJoined>
    {
        private readonly State _state;
        private readonly NetScope _scope;
        private readonly ISimulationService _simulations;

        public LockstepServer_UserSystem(State state, NetScope scope, ISimulationService simulations)
        {
            _state = state;
            _scope = scope;
            _simulations = simulations;
        }

        public override void Initialize(World world)
        {
            base.Initialize(world);

            _scope.Users.OnUserJoined += HandleUserJoined;
        }

        public SimulationEventResult Process(ISimulation simulation, UserJoined data)
        {
            var user = _scope.Peer!.Users.UpdateOrCreate(data.UserId, data.Claims);

            if (user.NetPeer is null)
            {
                return SimulationEventResult.Failure;
            }

            var lastTickId = _state.LastTickId;

            _scope.Messages.Create(new StateBegin())
                .AddRecipient(user.NetPeer)
                .Enqueue();

            foreach (Tick tick in _state.History)
            {
                if (tick.Id > lastTickId)
                {
                    break;
                }

                _scope.Messages.Create(new StateTick()
                {
                    Tick = tick
                }).AddRecipient(user.NetPeer).Enqueue();
            }

            _scope.Messages.Create(new StateEnd()
            {
                LastTickId = lastTickId
            }).AddRecipient(user.NetPeer).Enqueue();

            return SimulationEventResult.Success;
        }

        private void HandleUserJoined(IUserService sender, User args)
        {
            _simulations.Input(new UserJoined()
            {
                Key = UserJoined.ParallelType.Create(args.Id),
                Sender = ParallelKeys.System,
                UserId = args.Id,
                Claims = args.Where(x => x.Accessibility == ClaimAccessibility.Public).ToArray()
            });
        }
    }
}
