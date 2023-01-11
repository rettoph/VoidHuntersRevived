using Guppy.Network.Enums;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Services
{
    public interface ILockstepEventService
    {
        void Initialize(Action<ISimulationData, Confidence> publisher);

        void Publish(ISimulationData data, Confidence type);
    }
}
