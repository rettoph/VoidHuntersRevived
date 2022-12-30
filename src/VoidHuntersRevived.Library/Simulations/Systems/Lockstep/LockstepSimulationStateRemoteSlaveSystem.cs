using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Attributes;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Simulations.EventTypes;

namespace VoidHuntersRevived.Library.Simulations.Systems.Lockstep
{
    [NetAuthorizationFilter(NetAuthorization.Slave)]
    internal sealed class LockstepSimulationStateRemoteSlaveSystem : ILockstepSimulationSystem,
        ISubscriber<INetIncomingMessage<SimulationStateTick>>,
        ISubscriber<INetIncomingMessage<SimulationStateEnd>>
    {
        private readonly SimulationState _state;

        public LockstepSimulationStateRemoteSlaveSystem(SimulationState state)
        {
            _state = state;

            _state.BeginRead();
        }

        public void Initialize(World world)
        {
            // throw new NotImplementedException();
        }

        public void Dispose()
        {
            // throw new NotImplementedException();
        }

        public void Process(in INetIncomingMessage<SimulationStateTick> message)
        {
            _state.Read(message.Body.Tick);
        }

        public void Process(in INetIncomingMessage<SimulationStateEnd> message)
        {
            _state.Read(Tick.Empty(message.Body.LastTickId));
            _state.EndRead();
        }
    }
}
