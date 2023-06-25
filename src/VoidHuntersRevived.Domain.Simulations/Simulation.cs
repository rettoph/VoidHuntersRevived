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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VoidHuntersRevived.Domain.Simulations
{
    public abstract class Simulation : ISimulation, IDisposable
    {
        private readonly DrawEngineGroups _drawEnginesGroups;
        private Queue<EventDto> _events;

        protected readonly EventPublishingService publisher;

        public readonly SimulationType Type;
        public readonly ISpace Space;
        public readonly World World;

        SimulationType ISimulation.Type => this.Type;
        ISpace ISimulation.Space => this.Space;
        IWorld ISimulation.World => this.World;
        IEntityService ISimulation.Entities => this.World.Entities;

        protected Simulation(
            SimulationType type,
            ISpaceFactory spaceFactory,
            IFilteredProvider filtered,
            IBus bus)
        {
            this.Type = type;
            this.Space = spaceFactory.Create();
            this.World = new World(bus, filtered, new SimulationState(this));

            this.publisher = new EventPublishingService(this.World.Engines.OfType<IEventEngine>());

            _drawEnginesGroups = new DrawEngineGroups(this.World.Engines.OfType<IStepEngine<GameTime>>());
            _events = new Queue<EventDto>();
        }

        public virtual void Initialize(ISimulationService simulations)
        {
            this.World.Initialize();

            foreach(ISimulationEngine<ISimulation> engine in this.World.Engines.OfType<ISimulationEngine<ISimulation>>())
            {
                engine.Initialize(this);
            }
        }

        public virtual void Dispose()
        {
            this.World.Dispose();
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
            this.World.Step(step);
        }

        protected abstract void Publish(EventDto data);

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
