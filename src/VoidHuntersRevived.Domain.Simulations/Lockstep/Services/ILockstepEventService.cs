using Guppy.Network.Enums;
using System.Diagnostics.Tracing;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Services
{
    public interface ILockstepEventService
    {
        void Initialize(Action<IData, DataSource> publisher);

        void Publish(IData data, DataSource source);
    }
}
