using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Components;
using VoidHuntersRevived.Common.Services;

namespace VoidHuntersRevived.Common.Systems
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

            foreach(SimulationType type in this.simulations.Types)
            {
                var aspect = _aspectBuilder.Clone().All(type.EntityComponentType).Build(_world);
                var subscription = new EntitySubscription(_world, aspect);
                _subscriptions.Add(type, subscription);
            }

            this.Initialize(world.ComponentMapper);
        }

        public abstract void Initialize(IComponentMapperService mapperService);
    }
}
