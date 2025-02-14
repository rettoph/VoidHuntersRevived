using System.Diagnostics.CodeAnalysis;
using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Logging.Common;
using Guppy.Core.Logging.Common.Services;
using Guppy.Core.Messaging.Common;
using Guppy.Game.Common;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Simulations
{
    public abstract partial class Strategy : Scene, IStrategy
    {
        private ILogger? _logger;
        private readonly IMessageBus _messageBus;
        private readonly Lazy<ILoggerService> _loggerService;
        private readonly ActionSequenceGroup<StepSequenceGroupEnum, Step> _stepActions;

        protected ILogger logger => this._logger ??= this._loggerService.Value.GetLogger(this.GetType());

        public readonly StrategyTypeEnum Type;
        public ISimulation Simulation { get; private set; } = null!;
        public IStepEventService Events { get; }

        public Step CurrentStep { get; private set; }

        StrategyTypeEnum IStrategy.Type => this.Type;

        protected Strategy(
            StrategyTypeEnum type,
            IGuppyScope scope,
            IStepEventService eventService,
            Lazy<ILoggerService> loggerService) : base(scope)
        {
            this._loggerService = loggerService;
            this._messageBus = scope.Resolve<IMessageBus>();
            this._stepActions = new ActionSequenceGroup<StepSequenceGroupEnum, Step>(false);

            this.Type = type;
            this.Events = eventService;

            this.CurrentStep = new Step();

            this.Enabled = false;
            this.Visible = false;
        }

        protected override void Initialize()
        {
            this.Simulation = this.Resolve<ISimulation>();

            base.Initialize();

            this._stepActions.Add([this.Step_PublishEvents]); // Special case - add the internal queue submission method
            this._stepActions.Add(this.Systems);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            while (this.TryGetNextStep(gameTime, out Step? step))
            {
                this.DoStep(step);
            }
        }

        protected abstract bool TryGetNextStep(GameTime realTime, [MaybeNullWhen(false)] out Step step);
        protected virtual void DoStep(Step step)
        {
            this.CurrentStep = step;

            this._stepActions.Invoke(step);
        }

        [SequenceGroup<StepSequenceGroupEnum>(StepSequenceGroupEnum.PublishEvents)]
        private void Step_PublishEvents(Step step)
        {
            this.Events.Flush();
        }

        protected virtual void Revert(Id<IStepEvent> id, IStepEvent data)
        {
            this.logger.Verbose("Reverting {EventName}, {EventId}", data.GetType().Name, id);
            data.Revert(id.Value, this._messageBus);
        }
    }
}