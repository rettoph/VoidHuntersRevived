using Guppy.Common.Helpers;
using MonoGame.Extended.Entities;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    internal sealed partial class ParallelEntityService : IParallelEntityService
    {
        private Dictionary<int, ParallelKey> _idToKey = new Dictionary<int, ParallelKey>();
        private EntityManager _entities;
        private ComponentMapper<ISimulation> _simulations;

        public IEnumerable<ParallelKey> Entities => _idToKey.Values;

        public event Action<ParallelKey, ISimulation>? EntityAdded;
        public event Action<ParallelKey, ISimulation>? EntityRemoved;
        public event Action<ParallelKey, ISimulation, BitVector32>? EntityChanged;

        public ParallelEntityService(World world)
        {
            _entities = world.EntityManager;

            _simulations = world.ComponentManager.GetMapper<ISimulation>();

            _entities.EntityAdded += this.HandleEntityAdded;
            _entities.EntityChanged += this.HandleEntityChanged;
            _entities.EntityRemoved += this.HandleEntityRemoved;
        }

        public int GetEntityId(ParallelKey entityKey, ISimulation simulation)
        {
            return simulation.GetEntityId(entityKey);
        }

        public ParallelKey GetEntityKey(int entityId)
        {
            return _idToKey[entityId];
        }

        public void Map(int entityId, ParallelKey key, ISimulation simulation)
        {
            simulation.Map(key, entityId);
            _idToKey.Add(entityId, key);
        }

        public void Unmap(ParallelKey key, ISimulation simulation, out int entityId)
        {
            entityId = simulation.GetEntityId(key);
            simulation.Unmap(key);
            _idToKey.Remove(entityId);
        }

        private void HandleEntityRemoved(int entityId)
        {
            if(this.EntityRemoved is null)
            {
                return;
            }

            if(!_idToKey.TryGetValue(entityId, out ParallelKey entityKey))
            {
                return;
            }

            ISimulation simulation = _simulations.Get(entityId);
            this.EntityRemoved.Invoke(entityKey, simulation);
            this.Unmap(entityKey, simulation, out _);
        }

        private void HandleEntityChanged(int entityId, BitVector32 oldBits)
        {
            if (this.EntityChanged is null)
            {
                return;
            }

            if (!_idToKey.TryGetValue(entityId, out ParallelKey entityKey))
            {
                return;
            }

            this.EntityChanged.Invoke(entityKey, _simulations.Get(entityId), oldBits);
        }

        private void HandleEntityAdded(int entityId)
        {
            if (this.EntityAdded is null)
            {
                return;
            }

            if (!_idToKey.TryGetValue(entityId, out ParallelKey entityKey))
            {
                return;
            }

            this.EntityAdded.Invoke(entityKey, _simulations.Get(entityId));
        }

        public bool TryGetComponentBits(ParallelKey key, ISimulation simulation, out BitVector32 componentBits)
        {
            if(simulation.TryGetEntityId(key, out int entityId))
            {
                componentBits = _entities.GetComponentBits(simulation.GetEntityId(key));
                return true;
            }

            componentBits = default!;
            return false;
        }
    }
}
