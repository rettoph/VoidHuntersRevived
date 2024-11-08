using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Tests.Common.Providers;

namespace VoidHuntersRevived.Tests.Common.Simulations
{
    public class SimulationMocker(ISimulation instance, IStrategyMocker[] strategies) : IDisposable
    {
        public readonly ISimulation Instance = instance;
        public readonly IStrategyMocker[] Strategies = strategies;

        public void Dispose()
        {
            this.Instance.Dispose();
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

        public T GetService<TStrategy, T>()
            where TStrategy : IStrategy
            where T : class
        {
            return this.Get<TStrategy>().Provider.Get<T>();
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

        public SimulationMocker Input(VhId sourceId, IInputData data, bool verified)
        {
            foreach (IStrategyMocker strategy in this.Strategies)
            {
                strategy.Input(sourceId, data, verified);
            }

            return this;
        }

        public SimulationMocker Input(IInputData data, bool verified)
        {
            foreach (IStrategyMocker strategy in this.Strategies)
            {
                strategy.Input(data, verified);
            }

            return this;
        }

        public SimulationMocker InputMany<T>(Func<int, T> generator, int count, int offset, bool verified)
            where T : IInputData
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
