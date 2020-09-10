using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace VoidHuntersRevived.Library.Events
{
    public enum ShipEventType : Byte
    {
        Direction,
        Target
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct ShipEventArgs
    {
        #region Public Attributes
        [FieldOffset(0)]
        public ShipEventType Type;

        [FieldOffset(1)]
        public ShipDirectionData DirectionData;

        [FieldOffset(1)]
        public Vector2 TargetData;
        #endregion
    }
}
