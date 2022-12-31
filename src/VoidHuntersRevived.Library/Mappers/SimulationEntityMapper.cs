using Guppy.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Simulations;

namespace VoidHuntersRevived.Library.Mappers
{
    public sealed partial class SimulationEntityMapper
    {
        private IDictionary<int, SimulationEntityMap> _ids;
        private IDictionary<SimulationType, IDictionary<int, SimulationEntityMap>> _entityIds;

        public SimulationEntityMapper()
        {
            
            _ids = new Dictionary<int, SimulationEntityMap>();
            _entityIds = EnumHelper.ToDictionary<SimulationType, IDictionary<int, SimulationEntityMap>>(
                x => new Dictionary<int, SimulationEntityMap>(),
                SimulationType.None);
        }

        public bool TryGetEntityId(int id, SimulationType type, out int entityId)
        {
            var simulationEntityIdMap = this.Get(id);
            entityId = simulationEntityIdMap[type];

            return entityId != SimulationEntityMap.EmptyEntityId;
        }

        public int GetEntityId(int id, SimulationType type)
        {
            var simulationEntityIdMap = this.Get(id);
            var entityId = simulationEntityIdMap[type];

            return entityId;
        }

        public int GetEntityId(SimulationType from, int entityId, SimulationType to)
        {
            return _entityIds[from][entityId][to];
        }
        
        public int GetId(SimulationType type, int entityId)
        {
            return _entityIds[type][entityId].Id;
        }

        public void Set(int id, SimulationType type, int entityId)
        {
            var simulationEntityIdMap = this.Get(id);
            var currentSimulationId = simulationEntityIdMap[type];

            if (entityId == SimulationEntityMap.EmptyEntityId)
            {
                this.Remove(type, currentSimulationId);
                return;
            }

            var typeDictionary = _entityIds[type];
            if (currentSimulationId != SimulationEntityMap.EmptyEntityId)
            {
                typeDictionary.Remove(currentSimulationId);
            }

            simulationEntityIdMap[type] = entityId;
            typeDictionary.Add(entityId, simulationEntityIdMap);
        }

        public void Remove(int id, SimulationType type)
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
            simulationEntityId[type] = SimulationEntityMap.EmptyEntityId;

            if(!simulationEntityId.Empty)
            {
                return;
            }

            // If we are still here then there is no longer an entity
            // with the represented entity loaded. Remove it entirely.
            _ids.Remove(simulationEntityId.Id);
        }

        private SimulationEntityMap Get(int id)
        {
            if (!_ids.TryGetValue(id, out var entityId))
            {
                entityId = new SimulationEntityMap(id);
                _ids[id] = entityId;
            }

            return entityId;
        }
    }
}
