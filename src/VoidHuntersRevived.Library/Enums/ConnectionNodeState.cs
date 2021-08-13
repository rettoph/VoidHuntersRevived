using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Enums
{
    /// <summary>
    /// Indicates the state of a particular <see cref="ConnectionNode"/>.
    /// The <see cref="ConnectionNode.State"/> value will automatically update
    /// as connection are created or destroyed.
    /// </summary>
    public enum ConnectionNodeState
    {
        /// <summary>
        /// Indicates that a <see cref="ConnectionNode"/> has no attachment. It is neither a 
        /// <see cref="ConnectionNodeState.Parent"/> or a <see cref="ConnectionNodeState.Child"/>.
        /// </summary>
        Estranged,

        /// <summary>
        /// Indicates that a <see cref="ConnectionNode"/> has a <see cref="ConnectionNodeState.Child"/> connected to it.
        /// This is similar to the deprecated female connection node type.
        /// </summary>
        Parent,

        /// <summary>
        /// Indicates that a <see cref="ConnectionNode"/> is attached to a <see cref="ConnectionNodeState.Parent"/>.
        /// This is similar to the deprecated male connection node type.
        /// </summary>
        Child
    }
}
