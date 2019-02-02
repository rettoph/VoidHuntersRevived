using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Enums
{
    /// <summary>
    /// Player objects can move using the Player.Move() method.
    /// This method requires a movement selector, using a value
    /// from this current enum
    /// </summary>
    public enum MovementType
    {
        GoForward = 1,
        TurnRight = 2,
        GoBackward = 3,
        TurnLeft = 4,
        StrafeRight = 5,
        StrafeLeft = 6
    }
}
