using Guppy.Core.Common.Attributes;
using Guppy.Core.Network.Common;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Events;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;
using VoidHuntersRevived.Domain.Simulations.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Systems.Lockstep
{
    public class LockstepServer_TickSystem(
        ILockstepStrategy strategy,
        ITickService tickService,
        INetScope<IStrategy> scope
    ) : ISceneSystem,
        ITickSystem,
        IEventSystem<UserJoined>
    {
        private readonly INetScope<IStrategy> _scope = scope;
        private readonly List<Tick> _history = [];
        private readonly ILockstepStrategy _strategy = strategy;
        private readonly ITickService _tickService = tickService;

        [SequenceGroup<EventSequenceGroupEnum>(EventSequenceGroupEnum.Process)]
        public void Process(in VhId id, UserJoined data)
        {
            IUser? user = this._scope.Group.Peer!.Users.UpdateOrCreate(data.UserDto);

            if (user.NetPeer is null)
            {
                return;
            }

            int currentTickId = this._tickService.LastTickId;

            this._scope.CreateMessage(new TickHistoryStart()
            {
                CurrentTickId = currentTickId
            }).AddRecipient(user.NetPeer);

            foreach (Tick tick in this._history)
            {
                if (tick.Id > currentTickId)
                {
                    break;
                }

                this._scope.CreateMessage(new TickHistoryItem()
                {
                    Tick = tick
                }).AddRecipient(user.NetPeer);
            }

            this._scope.CreateMessage(new TickHistoryEnd()
            {
                CurrentTickId = currentTickId
            }).AddRecipient(user.NetPeer);
        }

        [SequenceGroup<TickSequenceGroupEnum>(TickSequenceGroupEnum.PublishInputs)]
        public void Tick(Tick tick)
        {
            // Broadcast the current tick to all connected peers
            this._scope.CreateMessage(in tick)
                .AddRecipients(this._scope.Group.Users.Peers);

            if (tick.Inputs.Length == 0)
            {
                return;
            }

            this._history.Add(tick);
        }
    }
}