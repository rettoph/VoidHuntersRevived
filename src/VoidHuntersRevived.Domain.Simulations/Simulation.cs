using Guppy.Common;
using Guppy.Common.Extensions;
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
        private readonly SimulationState _state;

        protected readonly IBus bus;
        protected readonly EventPublishingService publisher;

        public readonly SimulationType Type;
        public readonly ISpace Space;
        public readonly IWorld World;

        SimulationType ISimulation.Type => this.Type;
        ISpace ISimulation.Space => this.Space;
        IWorld ISimulation.World => this.World;

        protected Simulation(
            SimulationType type,
            IWorldFactory worldFactory,
            ISpaceFactory spaceFactory,
            IBus bus)
        {
            _state = new SimulationState(this);

            this.Type = type;
            this.World = worldFactory.Create(_state);
            this.Space = spaceFactory.Create();

            this.bus = bus;
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

        protected abstract void Update(GameTime gameTime);

        void ISimulation.Update(GameTime gameTime)
        {
            this.Update(gameTime);
        }

        public abstract void Publish(EventDto data);

        public abstract void Enqueue(EventDto data);
    }
}
