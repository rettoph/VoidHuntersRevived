using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Enums
{
    [Flags]
    public enum Direction : byte
    {
        None = 0,
        Forward = 1,
        Right = 2,
        Backward = 4,
        Left = 8,
        TurnRight = 16,
        TurnLeft = 32
    }
}
