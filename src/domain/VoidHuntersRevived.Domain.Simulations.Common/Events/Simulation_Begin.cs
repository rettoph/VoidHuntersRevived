using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Domain.Simulations.Common.Events
{
    public class Simulation_Begin : IStepInput<Simulation_Begin>
    {
        public bool IsPredictable => false;

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<Simulation_Begin, VhId>.Instance.Calculate(in source);
        }
    }
}