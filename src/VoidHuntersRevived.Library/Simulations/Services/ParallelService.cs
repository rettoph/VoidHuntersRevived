using Guppy.Common.Helpers;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Library.Simulations.Services
{
    internal sealed partial class ParallelService : IParallelService
    {
        private IDictionary<ParallelKey, ParallelEntityMap> _parallelEntities;
        private IDictionary<int, ParallelEntityMap> _entityIds;

        public ParallelService()
        {
            
            _parallelEntities = new Dictionary<ParallelKey, ParallelEntityMap>();
            _entityIds = new Dictionary<int, ParallelEntityMap>();
        }

        public bool TryGetEntityIdFromKey(ParallelKey key, SimulationType type, out int id)
        {
            var simulationEntityIdMap = this.Get(key);
            id = simulationEntityIdMap[type];

            return id != ParallelEntityMap.EmptyEntityId;
        }

        public bool TryGetEntityId(int fromId, SimulationType toType, out int toId)
        {
            if (_entityIds.TryGetValue(fromId, out var map))
            {
                toId = map[toType];
                return true;
            }

            toId = default;
            return false;
        }

        public int GetIdFromKey(ParallelKey key, SimulationType type)
        {
            var simulationEntityIdMap = this.Get(key);
            var entityId = simulationEntityIdMap[type];

            return entityId;
        }

        public int GetId(int fromId, SimulationType toType)
        {
            return _entityIds[fromId][toType];
        }
        
        public ParallelKey GetKey(int id)
        {
            return _entityIds[id].Key;
        }

        public void Set(ParallelKey key, SimulationType type, int id)
        {
            var simulationEntityIdMap = this.Get(key);
            var currentSimulationId = simulationEntityIdMap[type];

            if (id == ParallelEntityMap.EmptyEntityId)
            {
                this.Remove(type, currentSimulationId);
                return;
            }

            if (currentSimulationId != ParallelEntityMap.EmptyEntityId)
            {
                _entityIds.Remove(currentSimulationId);
            }

            simulationEntityIdMap[type] = id;
            _entityIds.Add(id, simulationEntityIdMap);
        }

        public void Remove(ParallelKey key, SimulationType type)
        {
            var simulationEntityId = this.Get(key);

            this.Remove(type, simulationEntityId[type]);
        }

        public void Remove(SimulationType type, int id)
        {
            if(!_entityIds.Remove(id, out var map))
            {
                return;
            }

            // Reset simulation id.
            map[type] = ParallelEntityMap.EmptyEntityId;

            if(!map.Empty)
            {
                return;
            }

            // If we are still here then there is no longer an entity
            // with the represented entity loaded. Remove it entirely.
            _parallelEntities.Remove(map.Key);
        }

        private ParallelEntityMap Get(ParallelKey key)
        {
            if (!_parallelEntities.TryGetValue(key, out var map))
            {
                map = new ParallelEntityMap(key);
                _parallelEntities[key] = map;
            }

            return map;
        }
    }
}
