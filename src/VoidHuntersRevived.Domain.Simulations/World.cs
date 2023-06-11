using Guppy.Common;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Common.Providers;
using VoidHuntersRevived.Domain.Simulations.Abstractions;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Domain.Simulations
{
    internal sealed class World : IWorld
    {
        private readonly EntityTypeService _types;
        private readonly EntityService _entities;
        private readonly ComponentService _components;
        private readonly ISystem[] _systems;

        private readonly EnginesRoot _enginesRoot;
        private readonly SimpleEntitiesSubmissionScheduler _simpleEntitiesSubmissionScheduler;

        public IEntityService Entities => _entities;
        public IComponentService Components => _components;
        public IEntityTypeService Types => _types;
        public ISystem[] Systems => _systems;

        public World(EntityTypeService types, ISystem[] systems)
        {
            _simpleEntitiesSubmissionScheduler = new SimpleEntitiesSubmissionScheduler();
            _enginesRoot = new EnginesRoot(_simpleEntitiesSubmissionScheduler);

            _types = types;
            _entities = new EntityService(_types, _enginesRoot.GenerateEntityFactory(), _enginesRoot.GenerateEntityFunctions());
            _components = new ComponentService(_entities);
            _systems = systems.ToArray();
        }

        public void Dispose()
        {
        }

        public void Update(GameTime gameTime)
        {
            _simpleEntitiesSubmissionScheduler.SubmitEntities();
        }

        public void Initialize()
        {
            IEnumerable<IEngine> engines = Enumerable.Empty<IEngine>()
                .Concat(new IEngine[]
                {
                    _entities,
                    _components
                })
                .Concat(_systems.Select(x => new SystemEngine(this, x)))
                .Concat(BuildReactiveEngines(_entities, _systems));

            foreach (IEngine engine in engines)
            {
                _enginesRoot.AddEngine(engine);
            }
        }

        private static IEnumerable<IEngine> BuildReactiveEngines(EntityService entities, ISystem[] systems)
        {
            foreach (ISystem system in systems)
            {
                foreach (Type interfaceType in system.GetType().GetInterfaces())
                {
                    if (interfaceType.IsConstructedGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IReactiveSystem<>))
                    {
                        Type engineType = typeof(ReactiveEngine<>).MakeGenericType(interfaceType.GenericTypeArguments[0]);
                        object? engine = Activator.CreateInstance(engineType, entities, system);

                        yield return engine as IEngine ?? throw new Exception();
                    }
                }
            }
        }
    }
}
