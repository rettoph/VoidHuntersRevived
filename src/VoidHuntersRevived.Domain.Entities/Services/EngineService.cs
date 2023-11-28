using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.Collections;
using Guppy.Common.Extensions;
using Guppy.Common.Providers;
using Guppy.Common.Services;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Domain.Entities.Engines;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class EngineService : IEngineService, IDisposable
    {
        private readonly EnginesRoot _enginesRoot;
        private readonly SimpleEntitiesSubmissionScheduler _submission;
        private readonly IBulkSubscriptionService _bulkSubscriptionService;
        private readonly IFilteredProvider _filtered;
        private IEngine[] _engines;
        private IStepGroupEngine<Step> _stepEngines;

        public EnginesRoot Root => _enginesRoot;

        public EngineService(
            IBulkSubscriptionService bulkSubscriptionService, 
            IFilteredProvider filtered,
            EnginesRoot enginesRoot,
            SimpleEntitiesSubmissionScheduler simpleEntitiesSubmissionScheduler)
        {
            _bulkSubscriptionService = bulkSubscriptionService;
            _filtered = filtered;
            _submission = simpleEntitiesSubmissionScheduler;
            _enginesRoot = enginesRoot;
            _stepEngines = null!;
            _engines = null!;
        }

        public IEngineService Load(params IState[] states)
        {
            _engines = _filtered.Instances<IEngine>(states).ToArray();

            return this;
        }

        public void Initialize()
        {
            foreach (IEngine engine in _engines.Sequence(InitializeSequence.Initialize))
            {
                _bulkSubscriptionService.Subscribe(engine.Yield());

                _enginesRoot.AddEngine(engine);
            }

            _stepEngines = _engines.CreateSequencedStepEnginesGroup<Step, StepSequence>(StepSequence.Step);
        }

        public void Dispose()
        {
            _bulkSubscriptionService.Unsubscribe(_engines);
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
        }
    }
}
