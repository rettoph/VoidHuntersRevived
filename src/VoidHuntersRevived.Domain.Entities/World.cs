using Svelto.ECS.Schedulers;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Domain.Entities.Services;
using Guppy.Common;
using Guppy.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Abstractions;
using Guppy.Common.Extensions;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.EnginesGroups;
using VoidHuntersRevived.Domain.Entities.Engines;

namespace VoidHuntersRevived.Domain.Entities
{
    public sealed class World : IWorld, IDisposable
    {
        private readonly IBus _bus;
        private readonly EnginesRoot _enginesRoot;
        private readonly StepEnginesGroup _stepEngines;
        private readonly SimpleEntitiesSubmissionScheduler _simpleEntitiesSubmissionScheduler;
        private readonly EntityTypeService _entityTypes;
        private readonly EntityService _entities;
        private readonly EntitySerializationService _serialization;

        public IEntityService Entities => _entities;
        public IEntitySerializationService Serialization => _serialization;

        public IEngine[] Engines { get; private set; }

        public World(IBus bus, IFilteredProvider filtered, params IState[] states)
        {
            this.Engines = filtered.Instances<IEngine>(states).Sort().ToArray();

            _bus = bus;
            _simpleEntitiesSubmissionScheduler = new SimpleEntitiesSubmissionScheduler();
            _enginesRoot = new EnginesRoot(_simpleEntitiesSubmissionScheduler);

            _entityTypes = filtered.Get<EntityTypeService>().Instance;
            _entities = new EntityService(_entityTypes, _enginesRoot.GenerateEntityFactory(), _enginesRoot.GenerateEntityFunctions(), _simpleEntitiesSubmissionScheduler, this.Engines);
            _serialization = new EntitySerializationService(_entities, _entityTypes);

            _stepEngines = new StepEnginesGroup(this.Engines.OfType<IStepEngine<Step>>());
        }

        public void Initialize()
        {
            foreach (IEngine engine in this.Engines)
            {
                _enginesRoot.AddEngine(engine);
            }

            _enginesRoot.AddEngine(_entities);
            _enginesRoot.AddEngine(_serialization);

            _bus.SubscribeMany(this.Engines.OfType<ISubscriber>());
        }

        public void Dispose()
        {
            _bus.UnsubscribeMany(this.Engines.OfType<ISubscriber>());
        }

        public void Step(Step step)
        {
            _stepEngines.Step(step);

            this.Entities.Clean();
        }
    }
}
