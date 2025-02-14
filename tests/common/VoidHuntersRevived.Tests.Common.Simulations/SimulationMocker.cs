using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Tests.Common.Providers;

namespace VoidHuntersRevived.Tests.Common.Simulations
{
    public class SimulationMocker(Simulation instance, IStrategyMocker[] strategies) : IDisposable
    {
        public readonly Simulation Instance = instance;
        public readonly IStrategyMocker[] Strategies = strategies;
        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed == false)
            {
                if (disposing == true)
                {
                    this.Instance.Dispose();
                    foreach (IStrategyMocker strategy in this.Strategies)
                    {
                        strategy.Dispose();
                    }
                }

                this._disposed = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public IStrategyMocker<T> Get<T>()
            where T : IStrategy
        {
            return this.Strategies.OfType<IStrategyMocker<T>>().First();
        }

        public IEnumerable<IStrategyMocker<T>> GetAll<T>()
                    where T : IStrategy
        {
            return this.Strategies.OfType<IStrategyMocker<T>>();
        }

        public T Resolve<TStrategy, T>()
            where TStrategy : IStrategy
            where T : class
        {
            return this.Get<TStrategy>().Instance.Resolve<T>();
        }

        public SimulationMocker Update(TimeSpan interval, int count)
        {
            for (int i = 0; i < count; i++)
            {
                foreach (IStrategyMocker strategy in this.Strategies)
                {
                    strategy.Update(interval, 1);
                }
            }

            return this;
        }

        public SimulationMocker Input(VhId sourceId, IStepInput data, bool verified)
        {
            foreach (IStrategyMocker strategy in this.Strategies)
            {
                strategy.Input(sourceId, data, verified);
            }

            return this;
        }

        public SimulationMocker Input(IStepInput data, bool verified)
        {
            foreach (IStrategyMocker strategy in this.Strategies)
            {
                strategy.Input(data, verified);
            }

            return this;
        }

        public SimulationMocker Input<TStrategy>(VhId sourceId, IStepInput data, bool verified)
            where TStrategy : IStrategy
        {
            foreach (IStrategyMocker strategy in this.Strategies.OfType<IStrategyMocker<TStrategy>>())
            {
                strategy.Input(sourceId, data, verified);
            }

            return this;
        }

        public SimulationMocker Input<TStrategy>(IStepInput data, bool verified)
            where TStrategy : IStrategy
        {
            foreach (IStrategyMocker strategy in this.Strategies.OfType<IStrategyMocker<TStrategy>>())
            {
                strategy.Input(data, verified);
            }

            return this;
        }

        public SimulationMocker InputMany<T>(Func<int, T> generator, int count, int offset, bool verified)
            where T : IStepInput
        {
            for (int i = 0; i < count; i++)
            {
                this.Input(generator(i + offset), verified);
            }

            return this;
        }

        public Dictionary<IStrategyMocker, int> CalculateTotalEntities<T>()
            where T : unmanaged, IEntityComponent
        {
            return this.Strategies.ToDictionary(x => x, x => x.CalculateTotalEntities<T>());
        }

        public SimulationMocker RunCoroutine(TimeSpan interval, VhId coroutineId, Func<VhIdProvider, IStrategyMocker, IEnumerator<int>> coroutine)
        {
            var coroutines = this.Strategies.Select(x => (strategy: x, coroutines: coroutine(new VhIdProvider(coroutineId), x))).ToArray();

            bool running = false;
            do
            {
                running = false;
                foreach ((IStrategyMocker _strategy, IEnumerator<int> _coroutines) in coroutines)
                {
                    bool result = _coroutines.MoveNext();
                    if (result == true)
                    {
                        int count = _coroutines.Current;
                        _strategy.Update(interval, count);
                    }

                    running |= result;
                }
            } while (running == true);

            for (int i = 0; i < coroutines.Length; i++)
            {
                coroutines[i].coroutines.Dispose();
            }

            return this;
        }

        public SimulationMocker RunCoroutine(VhId coroutineId, Action<VhIdProvider, IStrategyMocker> coroutine)
        {
            foreach (IStrategyMocker strategy in this.Strategies)
            {
                coroutine(new VhIdProvider(coroutineId), strategy);
            }

            return this;
        }
    }
}