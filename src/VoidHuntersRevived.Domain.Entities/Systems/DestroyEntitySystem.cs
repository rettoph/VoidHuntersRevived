using Guppy.Attributes;
using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Enums;
using VoidHuntersRevived.Common.Systems;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    [GuppyFilter<IGameGuppy>()]
    internal sealed class DestroyEntitySystem : BasicSystem,
        ISimulationEventListener<DestroyEntity>
    {
        public DestroyEntitySystem()
        {
        }

        public SimulationEventResult Process(ISimulation simulation, DestroyEntity data)
        {
            simulation.DestroyEntity(data.Key);

            return SimulationEventResult.Success;
        }
    }
}
