using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Globals.Constants
{
    public static class PipeIds
    {
        public static readonly Guid PlayersPipeId = new Guid(new Byte[16] { (Byte)PipeType.Players, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
    }
}
