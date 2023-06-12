using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Events;
using VoidHuntersRevived.Common.Simulations.Systems;

namespace VoidHuntersRevived.Game.Systems
{
    [AutoLoad]
    internal sealed class UserSystem : BasicSystem,
        IEventSubscriber<UserJoined>
    {
        public void Process(in EventId id, UserJoined data)
        {
        }
    }
}
