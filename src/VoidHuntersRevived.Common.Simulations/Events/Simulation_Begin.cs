using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Common.Simulations.Events
{
    public class Simulation_Begin : IInputData
    {
        public VhId ShipVhId => throw new NotImplementedException();

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<Simulation_Begin, VhId>.Instance.Calculate(in source);
        }
    }
}
