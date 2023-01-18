using Guppy.Network.Enums;
using System.Diagnostics.Tracing;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Services
{
    public interface ILockstepInputService
    {
        void Input(ParallelKey user, IData data);
    }
}
