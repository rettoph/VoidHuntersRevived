using Guppy.Common;
using Guppy.Common.Extensions;
using Guppy.Resources.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.ECS;
using VoidHuntersRevived.Common.ECS.Factories;
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
            this.World = worldFactory.Create(new IState[]
            {
                 new SimulationState(this)
            });
            this.Space = spaceFactory.Create();
            this.CurrentTick = Tick.First();

            this.publisher = new EventPublishingService(this.World.Systems);
        }

        public virtual void Initialize(ISimulationService simulations)
        {
            this.World.Initialize();
        }

        public void Dispose()
        {
            this.World.Dispose();
        }

        public virtual void Update(GameTime gameTime)
        {
            while (this.CanStep(gameTime))
            {
                this.DoStep(gameTime);
            }

            if(this.TryGetNextTick(this.CurrentTick, out Tick? next))
            {
                this.DoTick(next);
            }
        }

        protected abstract bool CanStep(GameTime realTime);
        protected virtual void DoStep(GameTime realTime)
        {

        }

        protected abstract bool TryGetNextTick(Tick current, [MaybeNullWhen(false)] out Tick next);
        protected virtual void DoTick(Tick tick)
        {

        }

        public abstract void Publish(EventDto data);

        public abstract void Enqueue(EventDto data);
    }
}
