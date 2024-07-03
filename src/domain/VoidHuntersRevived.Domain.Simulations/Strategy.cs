using Guppy.Game.Common;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;
using Serilog;
using Svelto.ECS;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
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
        private readonly Lazy<IEngineService> _engines;
        private readonly Queue<EventDto> _enqueued;
        private readonly Dictionary<Type, EventPublisher> _publishers;

        private IStepGroupEngine<FrameStart> _frameStartEnginesGroup;
        private IStepGroupEngine<Frame> _frameEnginesGroup;
        private IStepGroupEngine<FrameEnd> _frameEndEnginesGroup;

        private readonly FrameStart _frameStart;
        private readonly Frame _frame;
        private readonly FrameEnd _frameEnd;

        protected ILogger logger => _logger.Value;

        public readonly StrategyTypeEnum Type;
        public ISimulation Simulation => _simulation.Value;
        public IEngineService Engines => _engines.Value;

        public Step CurrentStep { get; private set; }

        StrategyTypeEnum IStrategy.Type => this.Type;

        protected Strategy(
            StrategyTypeEnum type,
            Lazy<ISimulation> simulation,
            Lazy<IEngineService> engines,
            Lazy<ILogger> logger)
        {
            _simulation = simulation;
            _engines = engines;
            _logger = logger;
            _enqueued = new Queue<EventDto>();
            _publishers = new Dictionary<Type, EventPublisher>();

            this.Type = type;

            _frameStartEnginesGroup = null!;
            _frameEnginesGroup = null!;
            _frameEndEnginesGroup = null!;

            this.CurrentStep = new Step();

            _frameStart = new FrameStart();
            _frame = new Frame(_frameStart);
            _frameEnd = new FrameEnd(_frame);
        }

        public virtual void Initialize(ISimulation simulation)
        {
            this.Engines.Initialize(this);


            EventPublisher.PopulatePublishers(this.Engines, this.logger, _publishers);

            _frameStartEnginesGroup = this.Engines.All().CreateSequencedStepEnginesGroup<FrameStart, DrawSequence>(DrawSequence.Draw);
            _frameEnginesGroup = this.Engines.All().CreateSequencedStepEnginesGroup<Frame, DrawSequence>(DrawSequence.Draw);
            _frameEndEnginesGroup = this.Engines.All().CreateSequencedStepEnginesGroup<FrameEnd, DrawSequence>(DrawSequence.Draw, true);
        }

        public virtual void Dispose()
        {
            this.Engines.Dispose();
        }

        public override void Draw(GameTime realTime)
        {
            base.Draw(realTime);

            _frameStart.GameTime = realTime;
            _frameStartEnginesGroup.Step(_frameStart);

            _frame.GameTime = realTime;
            _frameEnginesGroup.Step(_frame);

            _frameEnd.GameTime = realTime;
            _frameEndEnginesGroup.Step(_frameEnd);
        }

        public override void Update(GameTime realTime)
        {
            base.Update(realTime);

            while (this.TryGetNextStep(realTime, out Step? step))
            {
                this.DoStep(step);
            }
        }

        protected abstract bool TryGetNextStep(GameTime realTime, [MaybeNullWhen(false)] out Step step);
        protected virtual void DoStep(Step step)
        {
            this.Engines.Step(step);
            while (_enqueued.TryDequeue(out EventDto? enqueued))
            {
                this.Publish(enqueued);
            }

            this.CurrentStep = step;
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
