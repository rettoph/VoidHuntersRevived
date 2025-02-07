using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Guppy.Core.Common.Systems;
using Guppy.Core.Network.Common;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Events;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;
using VoidHuntersRevived.Domain.Simulations.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Systems.Lockstep
{
    public class LockstepServer_TickSystem(
        INetScope<IStrategy> scope
    ) : ISceneSystem,
        IInitializeSystem<ILockstepStrategy>,
        IOnTickSystem,
        IEventSystem<UserJoined>
    {
        private readonly INetScope<IStrategy> _scope = scope;
        private readonly List<Tick> _history = [];
        private ILockstepStrategy _strategy = null!;

        [SequenceGroup<InitializeSequenceGroupEnum>(InitializeSequenceGroupEnum.Setup)]
        public void Initialize(ILockstepStrategy strategy)
        {
            this._strategy = strategy;
        }

        public void Process(VhId id, UserJoined data)
        {
            IUser? user = this._scope.Group.Peer!.Users.UpdateOrCreate(data.UserDto);

            if (user.NetPeer is null)
            {
                return;
            }

            int currentTickId = this._strategy.CurrentTick.Id;

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