using Guppy.Common;
using Guppy.Common.Extensions;
using Guppy.Resources.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Physics.Factories;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Services;
using Guppy.Common.Providers;
using Svelto.ECS.Schedulers;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Domain.Simulations.EnginesGroups;
using VoidHuntersRevived.Common.Entities.Services;
using Serilog;

namespace VoidHuntersRevived.Domain.Simulations
{
    public abstract class Simulation : ISimulation, IDisposable
    {
        private readonly DrawEngineGroups _drawEnginesGroups;
        private Queue<EventDto> _events;

        protected readonly ILogger logger;

        public readonly SimulationType Type;
        public readonly ISpace Space;
        public readonly IEngineService Engines;

        SimulationType ISimulation.Type => this.Type;
        ISpace ISimulation.Space => this.Space;
        IEngineService ISimulation.Engines => this.Engines;

        public IEntityService Entities => this.Engines.Entities;
        public IEntitySerializationService Serialization => this.Engines.Serialization;
        public IEventPublishingService Events => this.Engines.Events;

        protected Simulation(
            SimulationType type,
            ISpaceFactory spaceFactory,
            IEngineService engines,
            IBus bus,
            ILogger logger)
        {
            this.Type = type;
            this.Space = spaceFactory.Create();
            this.Engines = engines.Load(new SimulationState(this));

            this.logger = logger;

            _drawEnginesGroups = new DrawEngineGroups(this.Engines.OfType<IStepEngine<GameTime>>());
            _events = new Queue<EventDto>();
        }

        public virtual void Initialize(ISimulationService simulations)
        {
            this.Engines.Initialize();

            foreach(ISimulationEngine<ISimulation> engine in this.Engines.OfType<ISimulationEngine<ISimulation>>())
            {
                engine.Initialize(this);
            }
        }

        public virtual void Dispose()
        {
            this.Engines.Dispose();
        }

        public virtual void Draw(GameTime realTime)
        {
            _drawEnginesGroups.Step(realTime);
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
        }

        public abstract void Publish(EventDto data);

        public void Publish(VhId eventId, IEventData data)
        {
            this.Publish(new EventDto()
            {
                Id = eventId,
                Data = data
            });
        }

        protected virtual void Enqueue(EventDto @event)
        {
            _events.Enqueue(@event);
        }

        public virtual void Input(VhId eventId, IInputData data)
        {
            this.Publish(eventId, data);
        }
    }
}
