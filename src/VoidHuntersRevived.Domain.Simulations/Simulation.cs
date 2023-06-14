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
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Domain.Pieces.Services;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Domain.Simulations.EnginesGroups;

namespace VoidHuntersRevived.Domain.Simulations
{
    public abstract class Simulation : ISimulation, IDisposable
    {
        private readonly TickEngineGroup _tickEngines;

        protected readonly EventPublishingService publisher;

        public readonly SimulationType Type;
        public readonly ISpace Space;
        public readonly IPieceService Pieces;
        public readonly World World;

        SimulationType ISimulation.Type => this.Type;
        ISpace ISimulation.Space => this.Space;
        IWorld ISimulation.World => this.World;
        IPieceService ISimulation.Pieces => this.Pieces;

        public Tick CurrentTick { get; private set; }

        protected Simulation(
            SimulationType type,
            ISpaceFactory spaceFactory,
            IFilteredProvider filtered,
            IBus bus,
            PieceConfigurationService pieces)
        {
            this.Type = type;
            this.Space = spaceFactory.Create();
            this.World = new World(bus, filtered, new SimulationState(this));
            this.Pieces = new PieceService(pieces, this.World.Entities);
            this.CurrentTick = Tick.First();

            this.publisher = new EventPublishingService(this.World.Engines.OfType<IEventEngine>());

            _tickEngines = new TickEngineGroup(this.World.Engines.OfType<ITickEngine>());
        }

        public virtual void Initialize(ISimulationService simulations)
        {
            this.World.Initialize();

            foreach(ISimulationEngine system in this.World.Engines.OfType<ISimulationEngine>())
            {
                system.Initialize(this);
            }
        }

        public virtual void Dispose()
        {
            this.World.Dispose();
        }

        public virtual void Update(GameTime realTime)
        {
            while (this.TryGetNextStep(realTime, out Step? step))
            {
                this.DoStep(step);
            }

            if(this.TryGetNextTick(this.CurrentTick, out Tick? next))
            {
                this.DoTick(next);
            }
        }

        protected abstract bool TryGetNextStep(GameTime realTime, [MaybeNullWhen(false)] out Step step);
        protected virtual void DoStep(Step step)
        {
            this.World.Step(step);
        }

        protected abstract bool TryGetNextTick(Tick current, [MaybeNullWhen(false)] out Tick next);
        protected virtual void DoTick(Tick tick)
        {
            this.CurrentTick = tick;

            _tickEngines.Step(tick);

            foreach (EventDto @event in tick.Events)
            {
                this.Publish(@event);
            }
        }

        public abstract void Publish(EventDto data);

        public abstract void Enqueue(EventDto data);
    }
}
