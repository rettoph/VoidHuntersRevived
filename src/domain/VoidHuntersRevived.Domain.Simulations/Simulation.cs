using Autofac;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;
using Serilog;
using Svelto.ECS;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Extensions;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Utilities;

namespace VoidHuntersRevived.Domain.Simulations
{
    public abstract partial class Simulation : ISimulation, IDisposable
    {
        private readonly Queue<EventDto> _enqueued;
        private readonly Dictionary<Type, EventPublisher> _publishers;

        private IStepGroupEngine<FrameStart> _frameStartEnginesGroup;
        private IStepGroupEngine<Frame> _frameEnginesGroup;
        private IStepGroupEngine<FrameEnd> _frameEndEnginesGroup;

        private readonly FrameStart _frameStart;
        private readonly Frame _frame;
        private readonly FrameEnd _frameEnd;

        protected ILogger logger { get; private set; }
        protected IEngineService engines { get; private set; }

        public readonly SimulationType Type;
        public ILifetimeScope Scope { get; private set; }

        public VhId Id { get; }
        public Step CurrentStep { get; private set; }

        SimulationType ISimulation.Type => this.Type;
        ILifetimeScope ISimulation.Scope => this.Scope;

        protected Simulation(SimulationType type, ILifetimeScope scope)
        {
            _enqueued = new Queue<EventDto>();
            _publishers = new Dictionary<Type, EventPublisher>();

            this.Id = HashBuilder<Simulation, Guid, SimulationType>.Instance.Calculate(Guid.NewGuid(), type);
            this.Type = type;
            this.Scope = scope;

            this.logger = null!;
            this.engines = null!;

            _frameStartEnginesGroup = null!;
            _frameEnginesGroup = null!;
            _frameEndEnginesGroup = null!;

            this.CurrentStep = new Step();

            _frameStart = new FrameStart();
            _frame = new Frame(_frameStart);
            _frameEnd = new FrameEnd(_frame);
        }

        public virtual void Initialize(ISimulationService simulations)
        {
            this.logger = this.Scope.Resolve<ILogger>();
            this.engines = this.Scope.Resolve<IEngineService>();

            EventPublisher.PopulatePublishers(this.engines, this.logger, _publishers);

            this.engines.Initialize();

            this.engines.InitializeSimulationEngines(this);

            _frameStartEnginesGroup = this.engines.All().CreateSequencedStepEnginesGroup<FrameStart, DrawSequence>(DrawSequence.Draw);
            _frameEnginesGroup = this.engines.All().CreateSequencedStepEnginesGroup<Frame, DrawSequence>(DrawSequence.Draw);
            _frameEndEnginesGroup = this.engines.All().CreateSequencedStepEnginesGroup<FrameEnd, DrawSequence>(DrawSequence.Draw, true);
        }

        public virtual void Dispose()
        {
            this.engines.Dispose();

            this.Scope.Dispose();
        }

        public virtual void Draw(GameTime realTime)
        {
            _frameStart.GameTime = realTime;
            _frameStartEnginesGroup.Step(_frameStart);

            _frame.GameTime = realTime;
            _frameEnginesGroup.Step(_frame);

            _frameEnd.GameTime = realTime;
            _frameEndEnginesGroup.Step(_frameEnd);
        }

        public virtual void Update(GameTime realTime)
        {
            while (this.TryGetNextStep(realTime, out Step? step))
            {
                this.DoStep(step);
            }
        }

        protected abstract bool TryGetNextStep(GameTime realTime, [MaybeNullWhen(false)] out Step step);
        protected virtual void DoStep(Step step)
        {
            this.engines.Step(step);
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
            if (@event.Data.IsPrivate == true)
            {
                _enqueued.Enqueue(@event);
                return;
            }

            this.logger.Error("{ClassName}::{MethodName} - Failed to enqueue event {Id}; Type = {Type}, IsPrivate = {IsPrivate}", nameof(Simulation), nameof(Enqueue), @event.Id, @event.Data.GetType().GetFormattedName(), @event.Data.IsPrivate);
        }
    }
}
