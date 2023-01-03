using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Services
{
    public interface ISimulatedService
    {
        public bool TryGetEntityId(SimulatedId id, SimulationType type, out int entityId);

        public bool TryGetEntityId(SimulationType from, int fromEntityId, SimulationType to, out int toEntityId);

        public int GetEntityId(SimulatedId id, SimulationType type);

        public int GetEntityId(SimulationType from, int entityId, SimulationType to);

        public SimulatedId GetId(SimulationType type, int entityId);

        public void Set(SimulatedId id, SimulationType type, int entityId);

        public void Remove(SimulatedId id, SimulationType type);

        public void Remove(SimulationType type, int entityId);
    }
}
