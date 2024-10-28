using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Tests.Common.Simulations
{
    public class SimulationMocker(ISimulation instance, IStrategyMocker[] strategies) : IDisposable
    {
        public readonly GameTime GameTime = new();
        public readonly ISimulation Instance = instance;
        public readonly IStrategyMocker[] Strategies = strategies;

        public void Dispose()
        {
            this.Instance.Dispose();
        }
    }
}
