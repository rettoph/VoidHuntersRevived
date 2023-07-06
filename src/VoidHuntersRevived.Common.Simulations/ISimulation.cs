using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Entities;
using Autofac;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface ISimulation
    {
        SimulationType Type { get; }
        ILifetimeScope Scope { get; }

        void Initialize(ISimulationService simulations);

        void Draw(GameTime realTime);

        void Update(GameTime realTime);

        void Publish(EventDto @event);
        void Publish(VhId eventId, IEventData data);

        void Input(VhId eventId, IInputData data);
    }
}
