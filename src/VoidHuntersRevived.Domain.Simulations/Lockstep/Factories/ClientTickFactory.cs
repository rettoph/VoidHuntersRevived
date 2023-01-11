using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Factories
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
