using Guppy.Common;
using Guppy.Common.Extensions;
using Guppy.Common.Services;
using Svelto.ECS;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class EngineService : IEngineService, IDisposable
    {
        private readonly EnginesRoot _enginesRoot;
        private readonly IBulkSubscriptionService _bulkSubscriptionService;
        private IEngine[] _engines;
        private IStepGroupEngine<Step> _stepEngines;

        public EnginesRoot Root => _enginesRoot;

        public EngineService(
            IBulkSubscriptionService bulkSubscriptionService,
            IFiltered<IEngine> engines,
            EnginesRoot enginesRoot)
        {
            _bulkSubscriptionService = bulkSubscriptionService;
            _enginesRoot = enginesRoot;
            _stepEngines = null!;
            _engines = engines.Instances.ToArray();
        }

        public void Initialize()
        {
            foreach (IEngine engine in _engines.Sequence(InitializeSequence.Initialize))
            {
                _bulkSubscriptionService.Subscribe(engine.Yield());

                _enginesRoot.AddEngine(engine);

                if (engine is IEngineEngine engineEngine)
                {
                    engineEngine.Initialize(_engines);
                }
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
