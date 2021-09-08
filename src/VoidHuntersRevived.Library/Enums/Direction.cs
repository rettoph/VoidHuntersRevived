using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Enums
{
    /// <summary>
    /// A helper flags enum used
    /// to store directional data
    /// about the current ship.
    /// </summary>
    [Flags]
    public enum Direction : Byte
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
