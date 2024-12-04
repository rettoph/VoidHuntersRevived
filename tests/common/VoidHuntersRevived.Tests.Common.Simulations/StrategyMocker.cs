using Autofac;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Predictive;
using VoidHuntersRevived.Tests.Common.Extensions;

namespace VoidHuntersRevived.Tests.Common.Simulations
{
    public interface IStrategyMocker : IDisposable
    {
        IStrategy Instance { get; }
        ILifetimeScope Scope { get; }

        void Update(TimeSpan interval, int count);
        void Input(IInputData data, bool verified);
        void Input(VhId sourceId, IInputData data, bool verified);

        int CalculateTotalEntities<T>()
            where T : unmanaged, IEntityComponent;
    }

    public interface IStrategyMocker<out TStrategy> : IStrategyMocker
        where TStrategy : IStrategy
    {
        new TStrategy Instance { get; }
    }

    public class StrategyMocker<TStrategy> : IStrategyMocker<TStrategy>
        where TStrategy : IStrategy
    {
        private readonly GameTime _gameTime = new();
        private int _sourceIdGeneratorIndex = 0;
        private readonly List<EventDto> _inputs = [];

        public TStrategy Instance { get; }
        public ILifetimeScope Scope { get; }

        IStrategy IStrategyMocker.Instance => this.Instance;

        public StrategyMocker(IContainer container)
        {
            this.Scope = container.BeginLifetimeScope();
            this.Instance = this.Scope.Resolve<TStrategy>();
        }

        public void Input(VhId sourceId, IInputData data, bool verified)
        {
            if (this.Instance is PredictiveStrategy)
            {
                this.Instance.Input(sourceId, data);
                return;
            }

            if (verified == false)
            {
                return;
            }

            _inputs.Add(new EventDto()
            {
                SourceId = sourceId,
                Data = data
            });
        }

        public void Input(IInputData data, bool verified)
        {
            this.Input(HashBuilder<IStrategyMocker, int>.Instance.Calculate(_sourceIdGeneratorIndex++), data, verified);
        }

        public void Update(TimeSpan interval, int count)
        {
            for (int i = 0; i < count; i++)
            {
                _gameTime.Step(interval);

                if (this.Instance is ILockstepStrategy lockstep)
                {
                    if (lockstep.StepsSinceTick == lockstep.StepsPerTick)
                    {
                        TickBuffer ticks = this.Scope.Resolve<TickBuffer>();

                        ticks.TryEnqueue(Tick.Create(lockstep.CurrentTick.Id + 1, _inputs.ToArray()));
                        _inputs.Clear();
                    }
                }

                this.Instance.Update(_gameTime);
            }
        }
        public int CalculateTotalEntities<T>()
            where T : unmanaged, IEntityComponent
        {
            return this.Scope.Resolve<IEntityQueryService>().CalculateTotal<T>();
        }

        public void Dispose()
        {
            this.Scope.Dispose();
        }
    }
}
