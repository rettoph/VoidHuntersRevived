using System.Collections;
using Guppy.Core.Common;
using Guppy.Core.Common.Services;
using Guppy.Core.Messaging.Common;
using Guppy.Core.Messaging.Common.Services;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public sealed class EngineService(
        IScopedSystemService systemService,
        IFiltered<IEngine> engines,
        IBrokerService brokers,
        EnginesRoot enginesRoot) : IEngineService
    {
        private readonly IBrokerService _brokers = brokers;
        private readonly List<IEngine> _engines = systemService.GetAll<IEngine>().Concat(engines).Distinct().ToList();

        public EnginesRoot Root { get; } = enginesRoot;

        public void Initialize()
        {
            var test = this._engines.GroupBy(x => x.GetType()).OrderByDescending(x => x.Count()).First().ToList();
            if (test.Count > 1)
            {
                throw new Exception();
            }

            foreach (IEngine engine in this._engines)
            {
                if (engine is IBaseSubscriber subscriber)
                {
                    this._brokers.AddSubscribers(subscriber.Yield());
                }

                this.Root.AddEngine(engine);
            }
        }

        public void Dispose()
        {
            this._brokers.RemoveSubscribers(this._engines.OfType<IBaseSubscriber>());
            this.Root.Dispose();
        }

        public T Get<T>()
        {
            return (T)this._engines.Single(x => x is T);
        }

        public IEnumerator<IEngine> GetEnumerator()
        {
            return this._engines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._engines.GetEnumerator();
        }
    }
}