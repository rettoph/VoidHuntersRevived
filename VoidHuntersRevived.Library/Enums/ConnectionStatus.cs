using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Enums
{
    /// <summary>
    /// Represents the status of several different connection entites.
    /// These include TractorBeamConnection's and NodeConnection's.
    /// 
    /// Respectively found and utilized within 
    /// TractorBeamConnection.cs and NodeConnection.cs
    /// </summary>
    public enum ConnectionStatus
    {
        Initializing,
        Connecting,
        Connected,
        Disconnecting,
        Disconnected
    }
}
