using Guppy.Common;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Systems;

namespace VoidHuntersRevived.Common.Simulations.Systems
{
    public abstract class ParallelEntitySystem : BasicSystem, ISimulationSystem
    {
        private readonly AspectBuilder _aspectBuilder;

        private World _world;
        private EntitySubscription _subscription = null!;

        private readonly IDictionary<SimulationType, EntitySubscription> _subscriptions;

        protected readonly ISimulationService simulations;
        protected EntitySubscription subscription => _subscription;

        public readonly IReadOnlyDictionary<SimulationType, EntitySubscription> Entities;

        public ParallelEntitySystem(ISimulationService simulations, AspectBuilder aspectBuilder)
        {
            _world = default!;
            _subscriptions = new Dictionary<SimulationType, EntitySubscription>();
            _aspectBuilder = aspectBuilder;

            this.simulations = simulations;

            this.Entities = new ReadOnlyDictionary<SimulationType, EntitySubscription>(_subscriptions);
        }

        public override void Initialize(World world)
        {
            _world = world;
            _subscription = new EntitySubscription(_world, _aspectBuilder.Build(_world));

            if(_subscriptions.Any())
            {

            }

            foreach (ISimulation simulation in this.simulations.Instances)
            {
                var aspect = _aspectBuilder.Clone().All(simulation.EntityComponentType).Build(_world);
                var subscription = new EntitySubscription(_world, aspect);
                _subscriptions.Add(simulation.Type, subscription);
            }

            _world.EntityManager.EntityAdded += OnEntityAdded;
            _world.EntityManager.EntityRemoved += OnEntityRemoved;
            _world.EntityManager.EntityChanged += OnEntityChanged;

            this.Initialize(world.ComponentManager);
        }

        public virtual void Initialize(ISimulation simulation)
        {
            // 
        }

        public abstract void Initialize(IComponentMapperService mapperService);

        protected virtual void OnEntityChanged(int entityId, BitVector32 oldBits) { }
        protected virtual void OnEntityAdded(int entityId) { }
        protected virtual void OnEntityRemoved(int entityId) { }
    }
}
