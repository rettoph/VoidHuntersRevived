﻿namespace VoidHuntersRevived.Domain.Entities.Common.Enums
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
