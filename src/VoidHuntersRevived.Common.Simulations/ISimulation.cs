using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Entities.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface ISimulation
    {
        SimulationType Type { get; }
        IServiceScope Scope { get; }

        void Initialize(ISimulationService simulations);

        void Draw(GameTime realTime);

        void Update(GameTime realTime);

        void Publish(EventDto @event);
        void Publish(VhId eventId, IEventData data);

        void Input(VhId eventId, IInputData data);
    }
}
