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
        ISimulationEventListener<DestroyShipPartEntity>
    {
        public void Process(ISimulationEvent<DestroyShipPartEntity> @event)
        {
            @event.Simulation.DestroyEntity(@event.Body.ShipPartEntityKey);
        }
    }
}
