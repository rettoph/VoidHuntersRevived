using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface ISimulation
    {
        SimulationType Type { get; }
        ISpace Space { get; }

        IEntityService Entities { get; }
        IComponentService Components { get; }
        ISystem[] Systems { get; }

        Tick CurrentTick { get; }

        void Initialize(ISimulationService simulations);

        void Update(GameTime realTime);

        void Publish(EventDto @event);

        void Enqueue(EventDto @event);
    }
}
