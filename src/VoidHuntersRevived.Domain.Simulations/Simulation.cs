using Guppy.Common;
using Guppy.Common.Extensions;
using Guppy.Resources.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Factories;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Physics.Factories;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Domain.Simulations
{
    public abstract partial class Simulation : ISimulation, IDisposable
    {
        private Action<Tick>? _onTicks;

        protected readonly EventPublishingService publisher;

        public readonly SimulationType Type;
        public readonly ISpace Space;
        public readonly IWorld World;

        SimulationType ISimulation.Type => this.Type;
        ISpace ISimulation.Space => this.Space;
        IWorld ISimulation.World => this.World;

        public Tick CurrentTick { get; private set; }

        protected Simulation(
            SimulationType type,
            IWorldFactory worldFactory,
            ISpaceFactory spaceFactory)
        {
            this.Type = type;
            this.Space = spaceFactory.Create();
            this.World = worldFactory.Create(new SimulationState(this));
            this.CurrentTick = Tick.First();

            this.publisher = new EventPublishingService(this.World.Systems);

            foreach(ITickSystem subscriber in this.World.Systems.OfType<ITickSystem>())
            {
                _onTicks += subscriber.Tick;
            }
        }

        public virtual void Initialize(ISimulationService simulations)
        {
            this.World.Initialize();

            foreach (ISimulationSystem system in this.World.Systems.OfType<ISimulationSystem>())
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

        }

        protected abstract bool TryGetNextTick(Tick current, [MaybeNullWhen(false)] out Tick next);
        protected virtual void DoTick(Tick tick)
        {
            this.CurrentTick = tick;

            _onTicks?.Invoke(tick);

            foreach(EventDto @event in tick.Events)
            {
                this.Publish(@event);
            }
        }

        public abstract void Publish(EventDto data);

        public abstract void Enqueue(EventDto data);
    }
}
