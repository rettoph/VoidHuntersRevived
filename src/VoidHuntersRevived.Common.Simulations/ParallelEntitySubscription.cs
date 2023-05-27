using Guppy.Common;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Common.Simulations
{
    public class ParallelEntitySubscription
    {
        private readonly Bag<ParallelKey> _activeEntities;
        private readonly ISimulation _simulation;
        private readonly IParallelEntityService _entities;
        private readonly Aspect _aspect;
        private bool _rebuildActives;

        public ParallelEntitySubscription(ISimulation simulation, IParallelEntityService entities, Aspect aspect)
        {
            _simulation = simulation;
            _entities = entities;
            _aspect = aspect;
            _activeEntities = new Bag<ParallelKey>(128);
            _rebuildActives = true;

            _entities.EntityAdded += OnEntityAdded;
            _entities.EntityRemoved += OnEntityRemoved;
            _entities.EntityChanged += OnEntityChanged;
        }

        private void OnEntityAdded(ParallelKey entityKey, ISimulation? simulation)
        {
            if (this.IsInterested(entityKey))
            {
                _activeEntities.Add(entityKey);
            }
        }

        private void OnEntityRemoved(ParallelKey entityKey, ISimulation simulation) => _rebuildActives = true;
        private void OnEntityChanged(ParallelKey entityKey, ISimulation simulation, BitVector32 oldBits) => _rebuildActives = true;

        public void Dispose()
        {
            _entities.EntityAdded -= OnEntityAdded;
            _entities.EntityChanged -= OnEntityChanged;
            _entities.EntityRemoved -= OnEntityRemoved;
        }

        public Bag<ParallelKey> ActiveEntities
        {
            get
            {
                if (_rebuildActives)
                    RebuildActives();

                return _activeEntities;
            }
        }

        public bool IsInterested(ParallelKey key)
        {
            if(_entities.TryGetComponentBits(key, _simulation, out BitVector32 componentBits))
            {
                return _aspect.IsInterested(componentBits);
            }

            return false;
        }

        public bool IsInterested(BitVector32 bits)
        {
            return _aspect.IsInterested(bits);
        }

        private void RebuildActives()
        {
            _activeEntities.Clear();

            foreach (ParallelKey entity in _entities.Entities)
                OnEntityAdded(entity, null);

            _rebuildActives = false;
        }
    }
}
