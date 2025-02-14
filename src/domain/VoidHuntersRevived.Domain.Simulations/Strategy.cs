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

namespace VoidHuntersRevived.Domain.Simulations
{
    public abstract partial class Strategy : Scene, IStrategy
    {
        private ILogger? _logger;
        private readonly IMessageBus _messageBus;
        private readonly Lazy<ILoggerService> _loggerService;
        private readonly Queue<EnqueuedStepEvent> _enqueued;
        private readonly ActionSequenceGroup<StepSequenceGroupEnum, Step> _stepActions;
        private readonly bool _disposed = false;

        protected ILogger logger => this._logger ??= this._loggerService.Value.GetLogger(this.GetType());

        public readonly StrategyTypeEnum Type;
        public ISimulation Simulation { get; private set; } = null!;

        public Step CurrentStep { get; private set; }

        StrategyTypeEnum IStrategy.Type => this.Type;

        protected Strategy(
            StrategyTypeEnum type,
            IGuppyScope scope,
            Lazy<ILoggerService> loggerService) : base(scope)
        {
            this._loggerService = loggerService;
            this._messageBus = scope.Resolve<IMessageBus>();
            this._enqueued = new Queue<EnqueuedStepEvent>();
            this._stepActions = new ActionSequenceGroup<StepSequenceGroupEnum, Step>(false);

            this.Type = type;

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
            while (this._enqueued.TryDequeue(out EnqueuedStepEvent? enqueued))
            {
                this.logger.Verbose("Publishing enqueued {EventName}, {EventId}", enqueued.Data.GetType().Name, enqueued.Id);
                enqueued.Data.Publish(enqueued.Id.Value, this._messageBus);
            }
        }

        protected virtual void Revert(Id<IStepEvent> id, IStepEvent data)
        {
            this.logger.Verbose("Reverting {EventName}, {EventId}", data.GetType().Name, id);
            data.Revert(id.Value, this._messageBus);
        }

        public virtual void Publish(Id<IStepEvent> id, IStepEvent data)
        {
            this.logger.Verbose("Publishing {EventName}, {EventId}", data.GetType().Name, id);
            data.Publish(id.Value, this._messageBus);
        }

        public abstract void Input(EnqueuedStepInput input);

        public void Enqueue(EnqueuedStepEvent @event)
        {
            this.logger.Verbose("Enqueing {EventName}, {EventId}", @event.GetType().Name, @event.Id.Value);
            this._enqueued.Enqueue(@event);
        }
    }
}