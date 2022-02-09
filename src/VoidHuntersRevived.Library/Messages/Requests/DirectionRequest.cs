using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Messages.Requests
{
    /// <summary>
    /// Helper struct to contain a direction state. 
    /// This allows the passing of a direction changed 
    /// event's data through a single property.
    /// </summary>
    public class DirectionRequest : ConsolidableMessage
    {
        /// <summary>
        /// The direction flag whose <see cref="State"/> is defined.
        /// </summary>
        public Direction Direction { get; init; }

        /// <summary>
        /// The state of the current <see cref="Direction"/>.
        /// </summary>
        public Boolean State { get; init; }
    }
}
