using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Structs
{
    /// <summary>
    /// Helper struct to contain a direction state. 
    /// This allows the passing of a direction changed 
    /// event's data through a single property.
    /// </summary>
    public struct DirectionState
    {
        /// <summary>
        /// The direction flag whose <see cref="Value"/> is defined.
        /// </summary>
        public readonly Direction Direction;

        /// <summary>
        /// The state of the current <see cref="Direction"/>.
        /// </summary>
        public readonly Boolean Value;

        #region Constructor
        /// <summary>
        /// Create a new DirectionState instance.
        /// </summary>
        /// <param name="direction">The direction flag whose <see cref="state"/> is defined.</param>
        /// <param name="state">The state of the recieved <see cref="direction"/>.</param>
        public DirectionState(Direction direction, bool state)
        {
            this.Direction = direction;
            this.Value = state;
        }
        #endregion
    }
}
