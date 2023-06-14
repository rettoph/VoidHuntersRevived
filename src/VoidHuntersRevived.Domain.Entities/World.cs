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
        private readonly EntityConfigurationService _entityConfigurations;
        private readonly EntityService _entities;

        public IEntityService Entities => _entities;

        public IEngine[] Engines { get; private set; }

        public World(IBus bus, IFilteredProvider filtered, params IState[] states)
        {
            _bus = bus;
            _simpleEntitiesSubmissionScheduler = new SimpleEntitiesSubmissionScheduler();
            _enginesRoot = new EnginesRoot(_simpleEntitiesSubmissionScheduler);

            _entityConfigurations = filtered.Get<EntityConfigurationService>().Instance;
            _entities = new EntityService(_entityConfigurations, _enginesRoot.GenerateEntityFactory(), _enginesRoot.GenerateEntityFunctions());

            this.Engines = filtered.Instances<IEngine>(states).Sort().ToArray();
            _stepEngines = new StepEnginesGroup(this.Engines.OfType<IStepEngine<Step>>());
        }

        public void Initialize()
        {
            foreach (IEngine engine in this.Engines)
            {
                _enginesRoot.AddEngine(engine);
            }

            _bus.SubscribeMany(this.Engines.OfType<ISubscriber>());
        }

        public void Dispose()
        {
            _bus.UnsubscribeMany(this.Engines.OfType<ISubscriber>());
        }

        public void Step(Step step)
        {
            _simpleEntitiesSubmissionScheduler.SubmitEntities();

            _stepEngines.Step(step);
        }
    }
}
