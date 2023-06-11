using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Systems;

namespace VoidHuntersRevived.Common.Simulations.Systems
{
    public abstract class BasicSystem : ISimulationSystem
    {
        public IWorld World { get; private set; } = null!;
        public ISimulation Simulation { get; private set; } = null!;

        public virtual void Initialize(IWorld world)
        {
            this.World = world;
        }

        public void Initialize(ISimulation simulation)
        {
            this.Simulation = simulation;
        }

        public virtual void Dispose()
        {
        }
    }
}
