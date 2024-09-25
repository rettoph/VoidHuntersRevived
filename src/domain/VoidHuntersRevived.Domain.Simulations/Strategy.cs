using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Game.Common;
using Microsoft.Xna.Framework;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Utilities;

namespace VoidHuntersRevived.Domain.Simulations
{
    public abstract partial class Strategy : Scene, IStrategy, IDisposable
    {
        private readonly Lazy<ILogger> _logger;
        private readonly Lazy<ISimulation> _simulation;
        private readonly Lazy<IEngineService> _engineService;
        private readonly Queue<EventDto> _enqueued;
        private readonly Dictionary<Type, EventPublisher> _publishers;
        private readonly ActionSequenceGroup<OnDrawSequenceGroup, GameTime> _drawActions;
        private readonly ActionSequenceGroup<OnStepSequenceGroup, Step> _stepActions;

        protected ILogger logger => _logger.Value;

        public readonly StrategyTypeEnum Type;
        public ISimulation Simulation => _simulation.Value;
        public IEngineService Engines => _engineService.Value;

        public Step CurrentStep { get; private set; }

        StrategyTypeEnum IStrategy.Type => this.Type;

        protected Strategy(
            StrategyTypeEnum type,
            Lazy<ISimulation> simulation,
            Lazy<IEngineService> engineService,
            Lazy<ILogger> logger)
        {
            _simulation = simulation;
            _engineService = engineService;
            _logger = logger;
            _enqueued = new Queue<EventDto>();
            _publishers = new Dictionary<Type, EventPublisher>();
            _stepActions = new ActionSequenceGroup<OnStepSequenceGroup, Step>();
            _drawActions = new ActionSequenceGroup<OnDrawSequenceGroup, GameTime>();

            this.Type = type;

            this.CurrentStep = new Step();

            this.Enabled = false;
            this.Visible = false;
        }

        public virtual void Initialize(ISimulation simulation)
        {
            this.Engines.Initialize(this);

            EventPublisher.PopulatePublishers(this.Engines, this.logger, _publishers);

            _drawActions.Add(this.Engines);

            _stepActions.Add([this.Step_PublishEvents]); // Special case - add the internal queue submission method
            _stepActions.Add(this.Engines);

            // Call all engine initializers
            Type initializeDelegate = typeof(Action<>).MakeGenericType(this.GetType());
            DelegateSequenceGroup<OnInitializeSequenceGroup>.Invoke(this.Engines, initializeDelegate, [this]);
        }

        public virtual void Dispose()
        {
            this.Engines.Dispose();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _drawActions.Invoke(gameTime);
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

            _stepActions.Invoke(step);
        }

        [SequenceGroup<OnStepSequenceGroup>(OnStepSequenceGroup.PublishEvents)]
        private void Step_PublishEvents(Step step)
        {
            while (_enqueued.TryDequeue(out EventDto? enqueued))
            {
                this.Publish(enqueued);
            }
        }

        protected virtual void Revert(EventDto @event)
        {
            _publishers[@event.Data.GetType()].Revert(@event);
        }
        public virtual void Publish(EventDto @event)
        {
            _publishers[@event.Data.GetType()].Publish(@event);
        }

        public void Publish(VhId sourceId, IEventData data)
        {
            this.Publish(new EventDto()
            {
                SourceId = sourceId,
                Data = data
            });
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
            _enqueued.Enqueue(@event);
        }
    }
}
