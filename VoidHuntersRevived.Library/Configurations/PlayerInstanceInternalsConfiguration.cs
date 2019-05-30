using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Library.Configurations
{
    /// <summary>
    /// When a new player in initializing, a custom event
    /// with the PlayerInstanceInternalsConfiguration is
    /// fired. This allows any player drivers to bind to
    /// that event and override the default player internal
    /// values on an as needed bases
    /// 
    /// IE, the server driver can create a new tractor beam
    /// while the client driver does not
    /// </summary>
    public class PlayerInstanceInternalsConfiguration
    {
        public TractorBeam TractorBeam { get; set; } 
    }
}
