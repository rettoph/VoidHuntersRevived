using Lidgren.Network;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Networking.Collections;
using VoidHuntersRevived.Networking.Enums;

namespace VoidHuntersRevived.Networking.Interfaces
{
    public interface IPeer : IGroup
    {
        Int64 UniqueIdentifier { get; }

        GroupCollection Groups { get; }

        ILogger Logger { get; }

        void Start();

        NetOutgoingMessage CreateMessage(MessageTarget target);
    }
}
