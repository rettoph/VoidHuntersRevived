using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Enums
{
    [Flags]
    public enum ChainDirection
    {
        Up = 1,
        Down = 2,
        Both = ChainDirection.Up | ChainDirection.Down
    }
}
