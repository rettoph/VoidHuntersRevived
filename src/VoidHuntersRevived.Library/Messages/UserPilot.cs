using Guppy.Common;
using Guppy.Network.Identity;
using Guppy.Network.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Messages
{
    public class UserPilot : Message, ITickData
    {
        public UserAction User;

        public UserPilot(UserAction user)
        {
            this.User = user;
        }
    }
}
