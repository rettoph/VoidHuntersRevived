﻿using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Networking.Collections;
using VoidHuntersRevived.Networking.Enums;

namespace VoidHuntersRevived.Networking.Interfaces
{
    public interface IPeer : IGroup
    {
        GroupCollection Groups { get; }

        void Start();

        NetOutgoingMessage CreateMessage(MessageTarget target);
    }
}