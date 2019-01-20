using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Core.Enums
{
    public enum InitializationState
    {
        UnInitialized,
        Booting,
        Preinitializing,
        Initializing,
        PostInitializing,
        Ready
    }
}
