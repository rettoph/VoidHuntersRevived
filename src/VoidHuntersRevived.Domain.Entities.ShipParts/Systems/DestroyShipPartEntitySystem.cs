using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts.Events;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Systems;

namespace VoidHuntersRevived.Domain.Entities.ShipParts.Systems
{
    internal sealed class DestroyShipPartEntitySystem : BasicSystem,
        ISubscriber<ISimulationEvent<DestroyShipPartEntity>>
    {
        public void Process(in ISimulationEvent<DestroyShipPartEntity> message)
        {
            message.Simulation.DestroyEntity(message.Body.ShipPartEntityKey);
        }
    }
}
