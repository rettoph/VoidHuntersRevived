using Guppy.Core.Common;
using Guppy.Core.Common.Services;
using Guppy.Core.Common.Systems;
using Guppy.Core.Logging.Common;
using Guppy.Tests.Common;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Tests.Common.Extensions;
using VoidHuntersRevived.Tests.Common.Simulations.Interfaces;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mocks
{
    public abstract class BaseStrategyMocker<TStrategy> : IBaseStrategyMocker<TStrategy>
        where TStrategy : Strategy
    {
        private TStrategy? _strategy;
        private IScopedSystem[]? _systems;

        public GameTime GameTime { get; set; }
        public Mocker<ISimulation> SimulationMocker { get; set; }
        public Mocker<IGuppyScope> GuppyScopeMocker { get; set; }
        public Mocker<IScopedSystemService> ScopedSystemServiceMocker { get; set; }
        public Mocker<ILogger> LoggerMocker { get; set; }
        public List<Func<TStrategy, IScopedSystem>> SystemFactories { get; set; }

        public TStrategy Strategy => this._strategy ??= this.Build();

        TStrategy IBaseStrategyMocker<TStrategy>.Strategy => this.Strategy;
        Strategy IBaseStrategyMocker.Strategy => this.Strategy;

        public BaseStrategyMocker()
        {
            this.GameTime = new();
            this.SystemFactories = [];
            this.GuppyScopeMocker = new Mocker<IGuppyScope>();
            this.ScopedSystemServiceMocker = new Mocker<IScopedSystemService>();
            this.SimulationMocker = new Mocker<ISimulation>();
            this.LoggerMocker = new Mocker<ILogger>();

            this.GuppyScopeMocker.Setup(x => x.Systems, this.ScopedSystemServiceMocker.GetInstance());

            this.ScopedSystemServiceMocker.Setup(
                expression: x => x.GetAll(),
                result: () => this._systems ??= this.SystemFactories.Select(x => x(this.Strategy)).ToArray());
        }

        protected abstract TStrategy Build();

        public void Update(TimeSpan interval, int count)
        {
            for (int i = 0; i < count; i++)
            {
                this.GameTime.Step(interval);

                this.Strategy.Update(this.GameTime);
            }
        }
    }
}
