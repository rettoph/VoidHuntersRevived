using Guppy.Core.Common.Attributes;
using Guppy.Core.Network.Common;
using Guppy.Core.Network.Common.Attributes;
using Guppy.Core.Network.Common.Enums;
using Serilog;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Events;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Engines.Lockstep
{
    [AutoLoad]
    [PeerFilter(PeerType.Server)]
    [StrategyFilter(StrategyTypeEnum.Lockstep)]
    [SequenceGroup<EngineSequence>(EngineSequence.Group03)]
    internal class LockstepServer_TickEngine : StrategyEngine<ILockstepStrategy>,
        IOnTickEngine,
        IEventEngine<UserJoined>
    {
        private readonly INetScope<IStrategy> _scope;
        private readonly List<Tick> _history;
        private readonly ILogger _logger;

        public LockstepServer_TickEngine(ILogger logger, INetScope<IStrategy> scope)
        {
            _scope = scope;
            _history = new List<Tick>();
            _logger = logger;
        }

        public string name { get; } = nameof(LockstepServer_TickEngine);

        public void Process(VhId id, UserJoined data)
        {
            IUser? user = _scope.Group.Peer!.Users.UpdateOrCreate(data.UserDto);

            if (user.NetPeer is null)
            {
                return;
            }

            var currentTickId = this.Strategy.CurrentTick.Id;

            _scope.CreateMessage(new TickHistoryStart()
            {
                CurrentTickId = currentTickId
            }).AddRecipient(user.NetPeer);

            foreach (Tick tick in _history)
            {
                if (tick.Id > currentTickId)
                {
                    break;
                }

                _scope.CreateMessage(new TickHistoryItem()
                {
                    Tick = tick
                }).AddRecipient(user.NetPeer);
            }

            _scope.CreateMessage(new TickHistoryEnd()
            {
                CurrentTickId = currentTickId
            }).AddRecipient(user.NetPeer);
        }

        [SequenceGroup<TickEngineSequenceGroup>(TickEngineSequenceGroup.PublishEvents)]
        public void OnTick(Tick tick)
        {
            // Broadcast the current tick to all connected peers
            _scope.CreateMessage(in tick)
                .AddRecipients(_scope.Group.Users.Peers);

            if (tick.Events.Length == 0)
            {
                return;
            }

            _history.Add(tick);
        }
    }
}
