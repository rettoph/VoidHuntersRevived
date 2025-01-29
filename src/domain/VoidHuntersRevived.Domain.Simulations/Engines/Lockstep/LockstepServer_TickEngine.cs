using Guppy.Core.Common.Attributes;
using Guppy.Core.Network.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Events;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Engines.Lockstep
{
    public class LockstepServer_TickEngine(INetScope<IStrategy> scope) : StrategyEngine<ILockstepStrategy>,
        IOnTickEngine,
        IEventEngine<UserJoined>
    {
        private readonly INetScope<IStrategy> _scope = scope;
        private readonly List<Tick> _history = [];

        public void Process(VhId id, UserJoined data)
        {
            IUser? user = this._scope.Group.Peer!.Users.UpdateOrCreate(data.UserDto);

            if (user.NetPeer is null)
            {
                return;
            }

            int currentTickId = this.Strategy.CurrentTick.Id;

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

        [SequenceGroup<OnTickSequenceGroupEnum>(OnTickSequenceGroupEnum.PublishEvents)]
        public void OnTick(Tick tick)
        {
            // Broadcast the current tick to all connected peers
            this._scope.CreateMessage(in tick)
                .AddRecipients(this._scope.Group.Users.Peers);

            if (tick.Events.Length == 0)
            {
                return;
            }

            this._history.Add(tick);
        }
    }
}