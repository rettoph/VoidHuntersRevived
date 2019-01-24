using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Networking.Enums
{
    /// <summary>
    /// Reference whether the message should be interpreted by
    /// peer object or a self contained network group
    /// </summary>
    public enum MessageTarget
    {
        Peer,
        Group
    }
}
