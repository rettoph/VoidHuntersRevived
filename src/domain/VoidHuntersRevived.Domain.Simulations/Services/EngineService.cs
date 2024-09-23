using Guppy.Core.Common;
using Guppy.Core.Common.Extensions;
using Guppy.Core.Messaging.Common;
using Guppy.Core.Messaging.Common.Services;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    public sealed class EngineService : IEngineService
    {
        private readonly IBrokerService _brokers;
        private readonly EnginesRoot _enginesRoot;
        private readonly EntitiesSubmissionScheduler _scheduler;
        private readonly Lazy<IFiltered<IEngineProvider>> _engineProviders;
        private List<IEngine> _engines;
        private IStepGroupEngine<Step> _stepEngines;

        public EnginesRoot Root => _enginesRoot;

        public EngineService(
            IFiltered<IEngine> engines,
            IBrokerService brokers,
            Lazy<IFiltered<IEngineProvider>> engineProviders,
            EnginesRoot enginesRoot,
            EntitiesSubmissionScheduler scheduler)
        {
            _brokers = brokers;
            _enginesRoot = enginesRoot;
            _engineProviders = engineProviders;
            _scheduler = scheduler;
            _stepEngines = null!;
            _engines = engines.ToList();
        }

        public void Initialize(IStrategy strategy)
        {
            _engines.AddRange(_engineProviders.Value.SelectMany(x => x.GetEngines()));
            _engines = _engines.Sequence<IEngine, EngineSequence>().ToList();

            foreach (IEngine engine in _engines)
            {
                if (engine is IBaseSubscriber subscriber)
                {
                    _brokers.AddSubscribers(subscriber.Yield());
                }

                _enginesRoot.AddEngine(engine);
            }

            _stepEngines = _engines.CreateSequencedStepEnginesGroup<Step, StepSequence>(StepSequence.Step);
        }

        public void Dispose()
        {
            _brokers.RemoveSubscribers(_engines.OfType<IBaseSubscriber>());
            _enginesRoot.Dispose();
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
