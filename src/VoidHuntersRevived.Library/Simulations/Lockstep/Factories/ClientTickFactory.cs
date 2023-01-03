using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Library.Common;

namespace VoidHuntersRevived.Library.Simulations.Lockstep.Factories
{
    [PeerTypeFilter(PeerType.Client)]
    internal sealed class ClientTickFactory : ITickFactory
    {
        public void Enqueue(ISimulationData data)
        {
            throw new NotImplementedException();
        }

        public Tick Create(int id)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
