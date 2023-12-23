using Autofac;
using Microsoft.Xna.Framework;
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

        void Publish(VhId sender, IEventData data);

        void Input(VhId sender, IInputData data);
    }
}
