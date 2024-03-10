using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Core.Utilities;

namespace VoidHuntersRevived.Common.Simulations.Events
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
