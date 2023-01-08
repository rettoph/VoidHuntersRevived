using MonoGame.Extended.Entities;
using System.Collections.ObjectModel;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Systems;

namespace VoidHuntersRevived.Common.Simulations.Systems
{
    public abstract class ParallelEntitySystem : BasicSystem
    {
        private readonly AspectBuilder _aspectBuilder;

        private World _world;

        private readonly IDictionary<SimulationType, EntitySubscription> _subscriptions;

        protected readonly ISimulationService simulations;

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

            foreach(ISimulation simulation in this.simulations.Instances)
            {
                var aspect = _aspectBuilder.Clone().All(simulation.EntityComponentType).Build(_world);
                var subscription = new EntitySubscription(_world, aspect);
                _subscriptions.Add(simulation.Type, subscription);
            }

            this.Initialize(world.ComponentMapper);
        }

        public abstract void Initialize(IComponentMapperService mapperService);
    }
}
