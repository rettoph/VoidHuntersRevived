using Guppy.Common;
using Guppy.Network.Identity;
using Guppy.Network.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Simulations.EventData;

namespace VoidHuntersRevived.Library.Messages
{
    public class UserPilot : Message, ISimulationEventData
    {
        public ushort PilotId { get; }
        public UserAction User { get; }

        public UserPilot(ushort pilotId, UserAction user)
        {
            this.PilotId = pilotId;
            this.User = user;
        }
    }
}
