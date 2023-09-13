using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Enums
{
    public enum AutoDisposeScope
    {
        /// <summary>
        /// No auto disposing
        /// </summary>
        None,

        /// <summary>
        /// Dispose the component when the entity is despawned
        /// </summary>
        Instance,

        /// <summary>
        /// Dispose the component when the EntityType is disposed
        /// </summary>
        Type
    }
}
