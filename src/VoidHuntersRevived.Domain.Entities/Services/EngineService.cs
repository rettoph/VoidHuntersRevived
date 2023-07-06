using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.Extensions;
using Guppy.Common.Providers;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Domain.Entities.EnginesGroups;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class EngineService : IEngineService, IDisposable
    {
        private readonly EnginesRoot _enginesRoot;
        private readonly SimpleEntitiesSubmissionScheduler _submission;
        private readonly IBus _bus;
        private readonly IFilteredProvider _filtered;
        private IEngine[] _engines;
        private StepEnginesGroup _stepEngines;

        public EnginesRoot Root => _enginesRoot;

        public IEventPublishingService Events { get; private set; }

        public IEntityService Entities { get; private set; }

        public IEntitySerializationService Serialization { get; private set; }

        public IFilterService Filters { get; private set; }

        public EngineService(
            IBus bus, 
            IFilteredProvider filtered,
            EnginesRoot enginesRoot,
            SimpleEntitiesSubmissionScheduler simpleEntitiesSubmissionScheduler)
        {
            _bus = bus;
            _filtered = filtered;
            _submission = simpleEntitiesSubmissionScheduler;
            _enginesRoot = enginesRoot;
            _stepEngines = null!;
            _engines = null!;

            this.Entities = null!;
            this.Serialization = null!;
            this.Events = null!;
            this.Filters = null!;
        }

        public IEngineService Load(params IState[] states)
        {
            _engines = _filtered.Instances<IEngine>(states).Sort().ToArray();

            this.Entities = this.Get<IEntityService>();
            this.Serialization = this.Get<IEntitySerializationService>();
            this.Events = this.Get<IEventPublishingService>();
            this.Filters = this.Get<IFilterService>();

            return this;
        }

        public void Initialize()
        {
            List<IStepEngine<Step>> stepEngines = new();

            foreach (IEngine engine in _engines)
            {
                if (engine is ISubscriber subscriber)
                {
                    _bus.Subscribe(subscriber);
                }

                if (engine is IStepEngine<Step> stepEngine)
                {
                    stepEngines.Add(stepEngine);
                }

                _enginesRoot.AddEngine(engine);
            }

            _stepEngines = new StepEnginesGroup(stepEngines);
        }

        public void Dispose()
        {
            _bus.UnsubscribeMany(this.OfType<ISubscriber>());
        }

        public IEnumerable<T> OfType<T>()
        {
            return _engines.OfType<T>();
        }

        public T Get<T>()
        {
            return (T)_engines.First(x => x is T);
        }

        public IEnumerable<IEngine> All()
        {
            return _engines;
        }

        public void Step(Step step)
        {
            _stepEngines.Step(step);

            _submission.SubmitEntities();

            this.Entities.Clean();
        }
    }
}
