using Guppy.Network.Enums;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Library.Simulations.Lockstep.Services
{
    public interface ILockstepEventPublishingService
    {
        void Initialize(Action<PeerType, ISimulationData> publisher);

        void Publish(PeerType source, ISimulationData data);
    }
}
