using Guppy.Core.Common;
using Guppy.Core.Messaging.Common;
using Guppy.Core.Messaging.Common.Services;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using System.Collections;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    public sealed class EngineService(
        IFiltered<IEngine> engines,
        IBrokerService brokers,
        Lazy<IFiltered<IEngineProvider>> engineProviders,
        EnginesRoot enginesRoot,
        EntitiesSubmissionScheduler scheduler) : IEngineService
    {
        private readonly IBrokerService _brokers = brokers;
        private readonly EnginesRoot _enginesRoot = enginesRoot;
        private readonly EntitiesSubmissionScheduler _scheduler = scheduler;
        private readonly Lazy<IFiltered<IEngineProvider>> _engineProviders = engineProviders;
        private List<IEngine> _engines = engines.ToList();

        public EnginesRoot Root => _enginesRoot;

        public void Initialize(IStrategy strategy)
        {
            _engines.AddRange(_engineProviders.Value.SelectMany(x => x.GetEngines()));

            foreach (IEngine engine in _engines)
            {
                if (engine is IBaseSubscriber subscriber)
                {
                    _brokers.AddSubscribers(subscriber.Yield());
                }

                _enginesRoot.AddEngine(engine);
            }
        }

        public void Dispose()
        {
            _brokers.RemoveSubscribers(_engines.OfType<IBaseSubscriber>());
            _enginesRoot.Dispose();
        }

        public T Get<T>()
        {
            return (T)_engines.Single(x => x is T);
        }

        public IEnumerator<IEngine> GetEnumerator()
        {
            return _engines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _engines.GetEnumerator();
        }
    }
}
