using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Domain.Simulations.Common.Events
{
    public class SimulationBegin : IStepInput<SimulationBegin>
    {
        public bool IsPredictable => false;

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<SimulationBegin, VhId>.Instance.Calculate(in source);
        }
    }
}