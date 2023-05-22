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
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    [GuppyFilter<IGameGuppy>()]
    [SimulationTypeFilter(SimulationType.Lockstep)]
    internal sealed class Lockstep_StateSystem : BasicSystem,
        ISubscriber<INetIncomingMessage<Tick>>,
        ISubscriber<INetIncomingMessage<StateBegin>>,
        ISubscriber<INetIncomingMessage<StateTick>>,
        ISubscriber<INetIncomingMessage<StateEnd>>

    {
        private readonly IState _state;

        public Lockstep_StateSystem(IFiltered<IState> state)
        {
            _state = state.Instance;
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

        public void Process(in INetIncomingMessage<Tick> message)
        {
            _state.Enqueue(message.Body);
        }
    }
}
