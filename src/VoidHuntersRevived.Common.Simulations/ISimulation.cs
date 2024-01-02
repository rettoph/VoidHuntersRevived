using Autofac;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface ISimulation
    {
        VhId Id { get; }
        SimulationType Type { get; }
        ILifetimeScope Scope { get; }

        void Initialize(ISimulationService simulations);

        void Draw(GameTime realTime);

        void Update(GameTime realTime);

        void Publish(EventDto @event);
        void Publish(VhId sourceId, IEventData data);

        void Input(VhId sourceId, IInputData data);
    }
}
