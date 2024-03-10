using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Domain.Simulations.Common.Events
{
    public class Simulation_Begin : IInputData
    {
        public bool IsPredictable => false;

        public VhId ShipVhId => default!;

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<Simulation_Begin, VhId>.Instance.Calculate(in source);
        }
    }
}
