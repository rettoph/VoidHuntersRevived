using Guppy.Common;
using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common.ECS;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface ISimulation
    {
        SimulationType Type { get; }
        ISpace Space { get; }
        IWorld World { get; }

        Tick CurrentTick { get; }

        void Initialize(ISimulationService simulations);

        void Update(GameTime gameTime);

        void Publish(EventDto @event);

        void Enqueue(EventDto @event);
    }
}
