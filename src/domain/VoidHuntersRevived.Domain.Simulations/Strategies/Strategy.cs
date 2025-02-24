using Guppy.Core.Common;
using Guppy.Core.Logging.Common;
using Guppy.Game.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;

namespace VoidHuntersRevived.Domain.Simulations.Strategies
{
    public abstract partial class Strategy : Scene, IStrategy
    {
        protected readonly ILogger logger;

        public readonly StrategyTypeEnum Type;
        public ISimulation Simulation { get; private set; } = null!;
        public IStepEventService Events { get; }

        StrategyTypeEnum IStrategy.Type => this.Type;

        protected Strategy(
            StrategyTypeEnum type,
            IGuppyScope scope,
            IStepEventService eventService,
            ILogger logger) : base(scope)
        {
            this.logger = logger;

            this.Type = type;
            this.Events = eventService;

            this.Enabled = false;
            this.Visible = false;
        }

        protected override void Initialize()
        {
            this.Simulation = this.Resolve<ISimulation>();

            base.Initialize();
        }
    }
}