using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Enums
{
    [Flags]
    public enum DirtyState
    {
        /// <summary>
        /// Indicates that the NetworkEntity should be cleaned once.
        /// Once cleaning has been started this flag will be disabled.
        /// </summary>
        Dirty = 1,
        
        /// <summary>
        /// Indicates that the NetworkEntity is always dirty and as
        /// soon as its done cleaning once it should attempt to clean
        /// again.
        /// </summary>
        Filthy = 2,

        /// <summary>
        /// Indicates that cleaning has been enqueued and once the Update
        /// message is created this flag will be diabled.
        /// </summary>
        Cleaning = 4,

        /// <summary>
        /// Contains both dirty and filthy flags.
        /// </summary>
        DirtyAndFilthy = Dirty | Filthy
    }
}
