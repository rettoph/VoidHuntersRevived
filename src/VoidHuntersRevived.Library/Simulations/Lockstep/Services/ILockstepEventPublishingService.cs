using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Library.Simulations.Lockstep.Services
{
    public interface ILockstepEventPublishingService
    {
        void Initialize(Action<PeerType, ISimulationData> publisher);

        void Publish(PeerType source, ISimulationData data);
    }
}
