using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Enums
{
    [Flags]
    public enum TreeComponent
    {
        /// <summary>
        /// Only clean the current node of the tree and nothing else
        /// </summary>
        Node = 1,

        /// <summary>
        /// Recursively push a clean event up the current tree to the root.
        /// </summary>
        Parent = 2,

        /// <summary>
        /// Recursively send a clean event down through all children of the 
        /// current sub-tree.
        /// </summary>
        Children = 4,
    }
}
