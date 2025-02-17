using Guppy.Core.Common;
using Guppy.Game.Common.Services;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;
using VoidHuntersRevived.Tests.Common.Extensions;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mocks
{
    public interface IStrategyAutoMock : IDisposable
    {
        IStrategy Instance { get; }

        void Update(TimeSpan interval, int count);
        void Input(IStepInput data, bool verified);
        void Input(VhId sourceId, IStepInput data, bool verified);

        int CalculateTotalEntities<T>()
            where T : unmanaged, IEntityComponent;
    }

    public interface IStrategyMocker<out TStrategy> : IStrategyAutoMock
        where TStrategy : IStrategy
    {
        new TStrategy Instance { get; }
    }

    public class StrategyMock<TStrategy> : IStrategyMocker<TStrategy>, IDisposable
        where TStrategy : class, IStrategy
    {
        private readonly GameTime _gameTime = new();
        private int _sourceIdGeneratorIndex = 0;
        private bool _disposed;

        public TStrategy Instance { get; }

        IStrategy IStrategyAutoMock.Instance => this.Instance;

        public StrategyMock(IGuppyScope parentScope, ISimulation simulation)
        {
            this.Instance = parentScope.Resolve<ISceneService>().Create<TStrategy>(builder =>
            {
                builder.RegisterInstance(simulation).As<ISimulation>();
            });
        }

        public void Input(VhId sourceId, IStepInput data, bool verified)
        {
            if (verified == false && this.Instance is ILockstepStrategy)
            {
                return;
            }

            this.Instance.Events.Input(sourceId, data);
        }

        public void Input(IStepInput data, bool verified)
        {
            this.Input(HashBuilder<IStrategyAutoMock, int>.Instance.Calculate(this._sourceIdGeneratorIndex++), data, verified);
        }

        public void Update(TimeSpan interval, int count)
        {
            for (int i = 0; i < count; i++)
            {
                this._gameTime.Step(interval);

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