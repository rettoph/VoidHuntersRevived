using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Logging.Common;
using Guppy.Core.Messaging.Common;
using Guppy.Game.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;

namespace VoidHuntersRevived.Domain.Simulations.Strategies
{
    public abstract partial class Strategy : Scene, IStrategy
    {
        private readonly IMessageBus _messageBus;
        private readonly ActionSequenceGroup<StepSequenceGroupEnum, Step> _stepActions;

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
            this._messageBus = scope.Resolve<IMessageBus>();
            this._stepActions = new ActionSequenceGroup<StepSequenceGroupEnum, Step>(false);

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

            this._stepActions.Add([this.Step_PublishEvents]); // Special case - add the internal queue submission method
            this._stepActions.Add(this.Systems.GetAll());
        }

        [SequenceGroup<StepSequenceGroupEnum>(StepSequenceGroupEnum.PublishEvents)]
        public void Step_PublishEvents(Step step)
        {
            this.Events.Flush();
        }
    }
}