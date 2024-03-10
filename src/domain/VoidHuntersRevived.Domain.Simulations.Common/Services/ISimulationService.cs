using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Simulations.Common.Services
{
    public interface ISimulationService
    {
        SimulationType Flags { get; }

        ReadOnlyCollection<ISimulation> Instances { get; }

        ISimulation this[SimulationType type] { get; }

        void Configure(SimulationType simulationTypeFlags);

        void Initialize();

        ISimulation First(params SimulationType[] types);

        void Draw(GameTime gameTime);

        void Update(GameTime gameTime);

        void Input(VhId sourceId, IInputData data);
    }
}
