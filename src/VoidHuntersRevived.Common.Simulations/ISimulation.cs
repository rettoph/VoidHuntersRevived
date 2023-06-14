using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface ISimulation
    {
        SimulationType Type { get; }
        ISpace Space { get; }
        IWorld World { get; }
        IPieceService Pieces { get; }

        Tick CurrentTick { get; }

        void Initialize(ISimulationService simulations);

        void Update(GameTime realTime);

        void Publish(EventDto @event);

        void Enqueue(EventDto @event);
    }
}
