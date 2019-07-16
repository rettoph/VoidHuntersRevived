using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.CustomEventArgs
{
    public class DirectionChangedEventArgs : ChangedEventArgs<Boolean>
    {
        public readonly Direction Direction;

        public DirectionChangedEventArgs(Direction direction, Boolean oldValue, Boolean newValue) : base(oldValue, newValue)
        {
            this.Direction = direction;
        }
    }
}
