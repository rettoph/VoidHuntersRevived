using Guppy.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Simulations;

namespace VoidHuntersRevived.Library.Services
{
    public sealed partial class SimulatedEntityIdService
    {
        private IDictionary<SimulatedId, SimulatedEntityMap> _ids;
        private IDictionary<SimulationType, IDictionary<int, SimulatedEntityMap>> _entityIds;

        public SimulatedEntityIdService()
        {
            
            _ids = new Dictionary<SimulatedId, SimulatedEntityMap>();
            _entityIds = EnumHelper.ToDictionary<SimulationType, IDictionary<int, SimulatedEntityMap>>(
                x => new Dictionary<int, SimulatedEntityMap>(),
                SimulationType.None);
        }

        public bool TryGetEntityId(SimulatedId id, SimulationType type, out int entityId)
        {
            var simulationEntityIdMap = this.Get(id);
            entityId = simulationEntityIdMap[type];

            return entityId != SimulatedEntityMap.EmptyEntityId;
        }

        public bool TryGetEntityId(SimulationType from, int fromEntityId, SimulationType to, out int toEntityId)
        {
            if (_entityIds[from].TryGetValue(fromEntityId, out var map))
            {
                toEntityId = map[to];
                return true;
            }

            toEntityId = default;
            return false;
        }

        public int GetEntityId(SimulatedId id, SimulationType type)
        {
            var simulationEntityIdMap = this.Get(id);
            var entityId = simulationEntityIdMap[type];

            return entityId;
        }

        public int GetEntityId(SimulationType from, int entityId, SimulationType to)
        {
            return _entityIds[from][entityId][to];
        }
        
        public SimulatedId GetId(SimulationType type, int entityId)
        {
            return _entityIds[type][entityId].Id;
        }

        public void Set(SimulatedId id, SimulationType type, int entityId)
        {
            var simulationEntityIdMap = this.Get(id);
            var currentSimulationId = simulationEntityIdMap[type];

            if (entityId == SimulatedEntityMap.EmptyEntityId)
            {
                this.Remove(type, currentSimulationId);
                return;
            }

            var typeDictionary = _entityIds[type];
            if (currentSimulationId != SimulatedEntityMap.EmptyEntityId)
            {
                typeDictionary.Remove(currentSimulationId);
            }

            simulationEntityIdMap[type] = entityId;
            typeDictionary.Add(entityId, simulationEntityIdMap);
        }

        public void Remove(SimulatedId id, SimulationType type)
        {
            var simulationEntityId = this.Get(id);

            this.Remove(type, simulationEntityId[type]);
        }

        public void Remove(SimulationType type, int entityId)
        {
            if(!_entityIds[type].Remove(entityId, out var simulationEntityId))
            {
                return;
            }

            // Reset simulation id.
            simulationEntityId[type] = SimulatedEntityMap.EmptyEntityId;

            if(!simulationEntityId.Empty)
            {
                return;
            }

            // If we are still here then there is no longer an entity
            // with the represented entity loaded. Remove it entirely.
            _ids.Remove(simulationEntityId.Id);
        }

        private SimulatedEntityMap Get(SimulatedId id)
        {
            if (!_ids.TryGetValue(id, out var entityId))
            {
                entityId = new SimulatedEntityMap(id);
                _ids[id] = entityId;
            }

            return entityId;
        }
    }
}
