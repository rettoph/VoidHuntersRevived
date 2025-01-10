using System.Diagnostics.CodeAnalysis;
using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Providers;
using Guppy.Game.Common;
using Microsoft.Xna.Framework;
using Serilog;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Utilities;

namespace VoidHuntersRevived.Domain.Simulations
{
    public abstract partial class Strategy : Scene, IStrategy, IDisposable
    {
        private ILogger? _logger;
        private readonly Lazy<ILoggerService> _loggerService;
        private readonly Lazy<IEngineService> _engineService;
        private readonly Queue<EventDto> _enqueued;
        private readonly Dictionary<Type, EventPublisher> _publishers;
        private readonly ActionSequenceGroup<OnDrawSequenceGroupEnum, GameTime> _drawActions;
        private readonly ActionSequenceGroup<OnStepSequenceGroupEnum, Step> _stepActions;
        private bool _disposed = false;

        protected ILogger logger => this._logger ??= this._loggerService.Value.GetOrCreate(this.GetType());

        public readonly StrategyTypeEnum Type;
        public ISimulation Simulation { get; private set; } = null!;
        public IEngineService Engines => this._engineService.Value;

        public Step CurrentStep { get; private set; }

        StrategyTypeEnum IStrategy.Type => this.Type;

        protected Strategy(
            StrategyTypeEnum type,
            Lazy<IEngineService> engineService,
            Lazy<ILoggerService> loggerService)
        {
            this._engineService = engineService;
            this._loggerService = loggerService;
            this._enqueued = new Queue<EventDto>();
            this._publishers = [];
            this._stepActions = new ActionSequenceGroup<OnStepSequenceGroupEnum, Step>(false);
            this._drawActions = new ActionSequenceGroup<OnDrawSequenceGroupEnum, GameTime>(true);

            this.Type = type;

            this.CurrentStep = new Step();

            this.Enabled = false;
            this.Visible = false;
        }

        public virtual void Initialize(ISimulation simulation)
        {
            this.Simulation = simulation;

            this.Engines.Initialize();

            EventPublisher.PopulatePublishers(this.Engines, this._loggerService.Value, this._publishers);

            this._drawActions.Add(this.Engines);

            this._stepActions.Add([this.Step_PublishEvents]); // Special case - add the internal queue submission method
            this._stepActions.Add(this.Engines);

            // Call all engine initializers
            Type initializeDelegate = typeof(Action<>).MakeGenericType(this.GetType());
            DelegateSequenceGroup<OnInitializeSequenceGroupEnum>.Invoke(this.Engines, initializeDelegate, false, [this]);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed == false)
            {
                if (disposing == true)
                {
                    this.Engines.Dispose();
                }

                this._disposed = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this._drawActions.Invoke(gameTime);
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

        protected virtual void Revert(EventDto @event) => this._publishers[@event.Data.GetType()].Revert(@event);
        public virtual void Publish(EventDto @event)
        {
            this.logger.Verbose("Publishing {EventName}, {EventId}", @event.Data.GetType().Name, @event.Id.Value);
            this._publishers[@event.Data.GetType()].Publish(@event);
        }

        public abstract void Input(VhId sourceId, IInputData data);

        public void Enqueue(VhId sourceId, IEventData data) => this.Enqueue(new EventDto()
        {
            SourceId = sourceId,
            Data = data
        });

        public void Enqueue(EventDto @event)
        {
            this.logger.Verbose("Enqueing {EventName}, {EventId}", @event.Data.GetType().Name, @event.Id.Value);
            this._enqueued.Enqueue(@event);
        }
    }
}