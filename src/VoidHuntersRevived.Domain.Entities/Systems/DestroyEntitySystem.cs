using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Systems;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    internal sealed class DestroyEntitySystem : BasicSystem,
        ISubscriber<IEvent<DestroyEntity>>
    {
        public DestroyEntitySystem()
        {
        }

        public void Process(in IEvent<DestroyEntity> message)
        {
            if(message.Sender == SimulationType.Predictive)
            {
                return;
            }

            message.Simulation.DestroyEntity(message.Data.Key);
        }
    }
}
