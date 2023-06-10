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
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Domain.Simulations.Events;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    [PeerTypeFilter(PeerType.Server)]
    [SimulationTypeFilter(SimulationType.Lockstep)]
    internal sealed class LockstepServer_UserSystem : BasicSystem,
        IEventSubscriber<UserJoined>
    {
        private readonly IGameState _state;
        private readonly NetScope _scope;
        private readonly ISimulationService _simulations;

        public LockstepServer_UserSystem(IFiltered<IGameState> state, NetScope scope, ISimulationService simulations)
        {
            _state = state.Instance;
            _scope = scope;
            _simulations = simulations;
        }

        public override void Initialize(IWorld world)
        {
            base.Initialize(world);

            _scope.Users.OnUserJoined += HandleUserJoined;
        }

        public void Process(in EventId id, UserJoined data)
        {
            User? user = _scope.Peer!.Users.UpdateOrCreate(data.UserId, data.Claims);

            if (user.NetPeer is null)
            {
                return;
            }

            int lastTickId = _state.Tick?.Id ?? 0;

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
