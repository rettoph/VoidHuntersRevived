using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations
{
    internal class InputRequest
    {
        public readonly SimulationInput Input;

        public InputRequest(SimulationInput input)
        {
            this.Input = input;
        }
    }
}
