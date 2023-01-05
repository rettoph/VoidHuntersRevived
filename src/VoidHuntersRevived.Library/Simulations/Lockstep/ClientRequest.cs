using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Library.Simulations.Lockstep
{
    internal sealed class ClientRequest : Message<ClientRequest>
    {
        public ClientRequest(ISimulationData data)
        {
            this.Data = data;
        }

        public ISimulationData Data { get; }
    }
}
