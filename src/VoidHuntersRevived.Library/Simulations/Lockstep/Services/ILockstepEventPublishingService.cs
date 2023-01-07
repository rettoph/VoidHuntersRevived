using Guppy.Network.Enums;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Library.Simulations.Lockstep.Services
{
    public interface ILockstepEventPublishingService
    {
        void Initialize(Action<SimulationType, ISimulationData> publisher);

        void Publish(SimulationType source, ISimulationData data);
    }
}
