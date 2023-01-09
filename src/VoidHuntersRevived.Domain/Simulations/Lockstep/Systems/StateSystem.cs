using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Systems
{
    [GuppyFilter<GameGuppy>()]
    [SimulationTypeFilter(SimulationType.Lockstep)]
    internal sealed class StateSystem : BasicSystem,
        ISubscriber<INetIncomingMessage<StateBegin>>,
        ISubscriber<INetIncomingMessage<StateTick>>,
        ISubscriber<INetIncomingMessage<StateEnd>>

    {
        private readonly State _state;

        public StateSystem(State state)
        {
            _state = state;
        }

        public void Process(in INetIncomingMessage<StateBegin> message)
        {
            _state.BeginRead();
        }

        public void Process(in INetIncomingMessage<StateTick> message)
        {
            _state.Read(message.Body.Tick);
        }

        public void Process(in INetIncomingMessage<StateEnd> message)
        {
            _state.EndRead(message.Body.LastTickId);
        }
    }
}
