using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Domain.Simulations.Events;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    [AutoLoad]
    internal sealed class UserSystem : BasicSystem,
        IEventSubscriber<UserJoined>
    {
        public void Process(in EventId id, UserJoined data)
        {
            Console.WriteLine("User Joined!");
        }
    }
}
