using Guppy.Network.Security;
using Guppy.Threading.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Server.Messages
{
    public sealed class UserJoinedMessage : IMessage
    {
        public User User { get; init; }
    }
}
