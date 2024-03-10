using Autofac;
using Guppy;
using Guppy.Extensions.Autofac;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;
using Serilog;
using Svelto.ECS;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Entities.Extensions;
using VoidHuntersRevived.Domain.Simulations.Utilities;

namespace VoidHuntersRevived.Domain.Simulations
{
    public abstract partial class Simulation : ISimulation, IDisposable
    {
        private readonly Queue<EventDto> _enqueued;
        private readonly Dictionary<Type, EventPublisher> _publishers;
        private IStepGroupEngine<GameTime> _drawEnginesGroup;

        protected readonly ILogger logger;

        public readonly SimulationType Type;
        public readonly IEngineService Engines;
        public readonly ITeamService Teams;
        public readonly ILifetimeScope Scope;

        public VhId Id { get; }
        public Step CurrentStep { get; private set; }

        SimulationType ISimulation.Type => this.Type;
        ILifetimeScope ISimulation.Scope => this.Scope;

        protected Simulation(SimulationType type, ILifetimeScope scope)
        {
            _enqueued = new Queue<EventDto>();

            this.Id = HashBuilder<Simulation, ulong, SimulationType>.Instance.Calculate(scope.Resolve<IGuppy>().Id, type);
            this.Type = type;

            this.Scope = scope.BeginGuppyScope(nameof(Simulation), builder =>
            {
                builder.RegisterInstance<ISimulation>(this);
            });

            // Pass the current scoped netscope to the new child scope
            this.Engines = this.Scope.Resolve<IEngineService>();
            this.Teams = this.Scope.Resolve<ITeamService>();

            // Build an event publisher dictionary
            this.logger = this.Scope.Resolve<ILogger>();
            this._publishers = EventPublisher.BuildPublishers(this.Engines, this.logger);

            _drawEnginesGroup = this.Engines.All().CreateSequencedStepEnginesGroup<GameTime, DrawSequence>(DrawSequence.Draw);

            this.CurrentStep = new Step();
        }

        public virtual void Initialize(ISimulationService simulations)
        {
            this.Engines.Initialize();

            this.Engines.InitializeSimulationEngines(this);
        }

        public virtual void Dispose()
        {
            this.Engines.Dispose();
        }

        public virtual void Draw(GameTime realTime)
        {
            _drawEnginesGroup.Step(realTime);
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
            if (@event.Data.IsPrivate == true)
            {
                _enqueued.Enqueue(@event);
                return;
            }

            this.logger.Error("{ClassName}::{MethodName} - Failed to enqueue event {Id}; Type = {Type}, IsPrivate = {IsPrivate}", nameof(Simulation), nameof(Enqueue), @event.Id, @event.Data.GetType().GetFormattedName(), @event.Data.IsPrivate);
        }
    }
}
