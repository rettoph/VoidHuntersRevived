using Guppy.Core.Common;
using Guppy.Game.Common.Services;
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

        void Update(TimeSpan interval, int count);
        void Input(IStepInput data, bool verified);
        void Input(VhId sourceId, IStepInput data, bool verified);

        int CalculateTotalEntities<T>()
            where T : unmanaged, IEntityComponent;
    }

    public interface IStrategyMocker<out TStrategy> : IStrategyMocker
        where TStrategy : IStrategy
    {
        new TStrategy Instance { get; }
    }

    public class StrategyMocker<TStrategy> : IStrategyMocker<TStrategy>, IDisposable
        where TStrategy : class, IStrategy
    {
        private readonly GameTime _gameTime = new();
        private int _sourceIdGeneratorIndex = 0;
        private bool _disposed;
        private readonly List<EnqueuedStepInput> _inputs = [];

        public TStrategy Instance { get; }

        IStrategy IStrategyMocker.Instance => this.Instance;

        public StrategyMocker(IGuppyScope parentScope, ISimulation simulation)
        {
            this.Instance = parentScope.Resolve<ISceneService>().Create<TStrategy>(builder =>
            {
                builder.RegisterInstance(simulation).As<ISimulation>();
            });
        }

        public void Input(VhId sourceId, IStepInput data, bool verified)
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

            this._inputs.Add(new EnqueuedStepInput(sourceId, data));
        }

        public void Input(IStepInput data, bool verified)
        {
            this.Input(HashBuilder<IStrategyMocker, int>.Instance.Calculate(this._sourceIdGeneratorIndex++), data, verified);
        }

        public void Update(TimeSpan interval, int count)
        {
            for (int i = 0; i < count; i++)
            {
                this._gameTime.Step(interval);

                if (this.Instance is ILockstepStrategy lockstep)
                {
                    if (lockstep.StepsSinceTick == lockstep.StepsPerTick)
                    {
                        TickBuffer ticks = this.Instance.Resolve<TickBuffer>();

                        ticks.TryEnqueue(Tick.Create(lockstep.CurrentTick.Id + 1, [.. this._inputs]));
                        this._inputs.Clear();
                    }
                }

                this.Instance.Update(this._gameTime);
            }
        }
        public int CalculateTotalEntities<T>()
            where T : unmanaged, IEntityComponent
        {
            return this.Instance.Resolve<IEntityQueryService>().CalculateTotal<T>();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed == false)
            {
                if (disposing == true)
                {
                    this.Instance.Dispose();
                }

                this._disposed = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}