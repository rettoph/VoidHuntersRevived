using System.Diagnostics.CodeAnalysis;
using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Logging.Common;
using Guppy.Core.Logging.Common.Services;
using Guppy.Game.Common;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Utilities;

namespace VoidHuntersRevived.Domain.Simulations
{
    public abstract partial class Strategy : Scene, IStrategy
    {
        private ILogger? _logger;
        private readonly Lazy<ILoggerService> _loggerService;
        private readonly Queue<EventDto> _enqueued;
        private readonly Dictionary<Type, EventPublisher> _publishers;
        private readonly ActionSequenceGroup<OnStepSequenceGroupEnum, Step> _stepActions;
        private bool _disposed = false;

        protected ILogger logger => this._logger ??= this._loggerService.Value.GetLogger(this.GetType());

        public readonly StrategyTypeEnum Type;
        public ISimulation Simulation { get; private set; } = null!;

        public Step CurrentStep { get; private set; }

        StrategyTypeEnum IStrategy.Type => this.Type;

        protected Strategy(
            StrategyTypeEnum type,
            Lazy<ILoggerService> loggerService)
        {
            this._loggerService = loggerService;
            this._enqueued = new Queue<EventDto>();
            this._publishers = [];
            this._stepActions = new ActionSequenceGroup<OnStepSequenceGroupEnum, Step>(false);

            this.Type = type;

            this.CurrentStep = new Step();

            this.Enabled = false;
            this.Visible = false;
        }

        public virtual void Initialize(ISimulation simulation)
        {
            this.Simulation = simulation;

            EventPublisher.PopulatePublishers(this.Systems, this._loggerService.Value, this._publishers);

            this._stepActions.Add([this.Step_PublishEvents]); // Special case - add the internal queue submission method
            this._stepActions.Add(this.Systems);

            // Call all engine initializers
            Type initializeDelegate = typeof(Action<>).MakeGenericType(this.GetType());
            DelegateSequenceGroup<OnInitializeSequenceGroupEnum>.Invoke(this.Systems, initializeDelegate, false, [this]);
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

        [SequenceGroup<OnStepSequenceGroupEnum>(OnStepSequenceGroupEnum.PublishEvents)]
        private void Step_PublishEvents(Step step)
        {
            while (this._enqueued.TryDequeue(out EventDto? enqueued))
            {
                this.Publish(enqueued);
            }
        }

        protected virtual void Revert(EventDto @event)
        {
            this._publishers[@event.Data.GetType()].Revert(@event);
        }

        public virtual void Publish(EventDto @event)
        {
            this.logger.Verbose("Publishing {EventName}, {EventId}", @event.Data.GetType().Name, @event.Id.Value);
            this._publishers[@event.Data.GetType()].Publish(@event);
        }

        public abstract void Input(VhId sourceId, IInputData data);

        public void Enqueue(VhId sourceId, IEventData data)
        {
            this.Enqueue(new EventDto()
            {
                SourceId = sourceId,
                Data = data
            });
        }

        public void Enqueue(EventDto @event)
        {
            this.logger.Verbose("Enqueing {EventName}, {EventId}", @event.Data.GetType().Name, @event.Id.Value);
            this._enqueued.Enqueue(@event);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this.Resolve<IGuppyScope>().Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                this._disposed = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Strategy()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}