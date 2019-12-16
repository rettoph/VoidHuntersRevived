using Guppy;
using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Library.Entities.Controllers;

namespace VoidHuntersRevived.Client.Library.Drivers.Utilities
{
    [IsDriver(typeof(Quarantine))]
    internal sealed class QuarantineDebugDriver : Driver<Quarantine>
    {
        public QuarantineDebugDriver(DebugOverlay debug, Quarantine driven) : base(driven)
        {
            debug.AddLine(gt => $"\nQuarantinees: {this.driven.Components.Count.ToString("#,##0")}");
        }
    }
}
