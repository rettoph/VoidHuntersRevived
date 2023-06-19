using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface ISimulation
    {
        SimulationType Type { get; }
        ISpace Space { get; }
        IWorld World { get; }
        IEntityService Entities { get; }

        void Initialize(ISimulationService simulations);

        void Draw(GameTime realTime);

        void Update(GameTime realTime);

        void Publish(EventDto @event);

        void Enqueue(EventDto @event);

        void Enqueue(VhId eventId, IEventData data);
    }
}
