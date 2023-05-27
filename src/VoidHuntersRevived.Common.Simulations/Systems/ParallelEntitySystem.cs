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

        private World _world = null!;
        private IParallelEntityService _entities = null!;
        private EntitySubscription _subscription = null!;

        private readonly IDictionary<SimulationType, ParallelEntitySubscription> _subscriptions;

        protected EntitySubscription subscription => _subscription;

        public readonly IReadOnlyDictionary<SimulationType, ParallelEntitySubscription> Entities;

        public ParallelEntitySystem(AspectBuilder aspectBuilder)
        {
            _subscriptions = new Dictionary<SimulationType, ParallelEntitySubscription>();
            _aspectBuilder = aspectBuilder;

            this.Entities = new ReadOnlyDictionary<SimulationType, ParallelEntitySubscription>(_subscriptions);
        }

        public override void Initialize(World world)
        {
            _world = world;

            _subscription = new EntitySubscription(_world, _aspectBuilder.Build(_world));
        }

        public virtual void Initialize(IParallelComponentMapperService components, IParallelEntityService entities)
        {
            _entities = entities;

            _entities.EntityAdded += OnEntityAdded;
            _entities.EntityRemoved += OnEntityRemoved;
            _entities.EntityChanged += OnEntityChanged;
        }

        public virtual void Initialize(ISimulation simulation)
        {
            Aspect aspect = _aspectBuilder.Clone().All(simulation.EntityComponentType).Build(_world);
            ParallelEntitySubscription subscription = new ParallelEntitySubscription(simulation, _entities, aspect);
            _subscriptions.Add(simulation.Type, subscription);
        }

        protected virtual void OnEntityChanged(ParallelKey entityKey, ISimulation simulation, BitVector32 oldBits) { }
        protected virtual void OnEntityAdded(ParallelKey entityKey, ISimulation simulation) { }
        protected virtual void OnEntityRemoved(ParallelKey entityKey, ISimulation simulation) { }
    }
}
