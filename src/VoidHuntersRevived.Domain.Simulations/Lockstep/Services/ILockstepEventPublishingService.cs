using Guppy.Network.Enums;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Services
{
    public interface ILockstepEventPublishingService
    {
        void Initialize(Action<ISimulationInputData, Confidence> publisher);

        void Publish(ISimulationInputData data, Confidence confidence);
    }
}
