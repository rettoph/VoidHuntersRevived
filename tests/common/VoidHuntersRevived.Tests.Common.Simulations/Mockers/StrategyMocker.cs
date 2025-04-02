using Guppy.Core.Common.Services;
using Guppy.Core.Common.Systems;
using Guppy.Core.Logging.Common;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Mockers;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Strategies;
using VoidHuntersRevived.Tests.Common.Extensions;
using VoidHuntersRevived.Tests.Common.Simulations.Interfaces;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mockers
{
    public abstract class StrategyMocker<TStrategy> : Builder<TStrategy>, IStrategyMocker<TStrategy>
        where TStrategy : Strategy
    {
        private IScopedSystem[]? _systems;

        public GameTime GameTime { get; set; }
        public ISimulation? Simulation { get; set; }
        public GuppyScopeMocker GuppyScopeMocker { get; set; }
        public Mocker<IScopedSystemService> ScopedSystemServiceMocker { get; set; }
        public Mocker<ILogger> LoggerMocker { get; set; }
        public List<Func<TStrategy, IScopedSystem>> SystemFactories { get; set; }
        public IScopedSystem[] Systems => this._systems ??= this.SystemFactories.Select(x => x(this.Strategy)).ToArray();

        public TStrategy Strategy => this.Object;

        public StrategyMocker()
        {
            this.GameTime = new();
            this.SystemFactories = [];
            this.Simulation = null;

            this.ScopedSystemServiceMocker = new Mocker<IScopedSystemService>()
                .SetupReturn(x => x.GetAll(), () => this.Systems);

            this.GuppyScopeMocker = new GuppyScopeMocker()
                .SetupReturn(x => x.Systems, () => this.ScopedSystemServiceMocker.Object)
                .SetupResolve<ISimulation>(() => this.Simulation ?? throw new NotImplementedException());

            this.LoggerMocker = new Mocker<ILogger>();
        }

        public void Update(TimeSpan interval, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                this.GameTime.Step(interval);

                this.Strategy.Update(this.GameTime);
            }
        }

        public void Publish(VhId sourceId, IStepEvent input)
        {
            this.Strategy.Events.Publish(sourceId, input);
        }

        public void Input(VhId sourceId, IStepInput input)
        {
            this.Strategy.Events.Input(sourceId, input);
        }

        public void Input(IStepInput input)
        {
            this.Input(VhId.NewVhId(), input);
        }

        public void Input<TInput>()
            where TInput : IStepInput, new()
        {
            this.Input(VhId.NewVhId(), new TInput());
        }
    }
}
