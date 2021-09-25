using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Enums
{
    [Flags]
    public enum ShipPartSerializationFlags
    {
        None = 0,

        /// <summary>
        /// Indicates the data stream contains necessary data to create the part
        /// if it is not found.
        /// </summary>
        Create = 1,

        /// <summary>
        /// When included it can be assumed that the data stream
        /// contains recursive data of the entire chain.
        /// </summary>
        Tree = 2,

        CreateTree = ShipPartSerializationFlags.Create | ShipPartSerializationFlags.Tree
    }
}
