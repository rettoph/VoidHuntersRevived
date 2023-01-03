using Guppy.Attributes;
using Guppy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Library.Simulations.Events;

namespace VoidHuntersRevived.Library.Simulations.Systems
{
    [GuppyFilter<GameGuppy>()]
    internal sealed class UserPilotSystem : BasicSystem,
        ISubscriber<ISimulationEvent<UserJoined>>
    {
        public UserPilotSystem()
        {

        }

        public void Process(in ISimulationEvent<UserJoined> message)
        {
            // throw new NotImplementedException();
        }
    }
}
