using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Common.Entities.Systems;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Physics.Systems
{
    internal sealed class SpaceSystem : BasicSystem, IStepSystem
    {
        public void Step(Step step)
        {
            this.Simulation.Space.Step(step);
        }
    }
}
