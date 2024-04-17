using Guppy.Core.Common;
using Guppy.Core.Common.Extensions;
using Guppy.Core.Messaging.Common.Services;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class EngineService : IEngineService, IDisposable
    {
        private readonly EnginesRoot _enginesRoot;
        private readonly SimpleEntitiesSubmissionScheduler _scheduler;
        private readonly IMagicBrokerService _brokers;
        private IFiltered<IEngine> _engines;
        private IStepGroupEngine<Step> _stepEngines;

        public EnginesRoot Root => _enginesRoot;

        public EngineService(
            IMagicBrokerService brokers,
            IFiltered<IEngine> engines,
            EnginesRoot enginesRoot,
            SimpleEntitiesSubmissionScheduler scheduler)
        {
            _brokers = brokers;
            _enginesRoot = enginesRoot;
            _scheduler = scheduler;
            _stepEngines = null!;
            _engines = engines;
        }

        public void Initialize()
        {
            foreach (IEngine engine in _engines.Sequence(InitializeSequence.Initialize))
            {
                _brokers.Subscribe(engine.Yield());

                _enginesRoot.AddEngine(engine);

                if (engine is IEngineEngine engineEngine)
                {
                    engineEngine.Initialize(this);
                }
            }

            _stepEngines = _engines.CreateSequencedStepEnginesGroup<Step, StepSequence>(StepSequence.Step);

            _scheduler.SubmitEntities();
        }

        public void Dispose()
        {
            _brokers.Unsubscribe(_engines);
        }

        public IEnumerable<T> OfType<T>()
        {
            return _engines.OfType<T>();
        }

        public T Get<T>()
        {
            return (T)_engines.Single(x => x is T);
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
