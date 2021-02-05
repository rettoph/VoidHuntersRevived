using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Enums
{
    public enum MessageType
    {
        /// <summary>
        /// Ran once when creating a network entity instance immediately 
        /// after PreInitialiation.
        /// </summary>
        Create = 0,

        /// <summary>
        /// Ran once when setting up a network entity. 
        /// Takes place after initialization is complete.
        /// </summary>
        Setup = 1,

        /// <summary>
        /// Sent as needed, should contain required entity update data.
        /// </summary>
        Update = 2,

        /// <summary>
        /// A micro update that contains chainges for a single attribute or value.
        /// This is managed within the NetworkEntity.Actions instance.
        /// </summary>
        Ping = 3,

        /// <summary>
        /// Sent once, when an entity is to be removed from the game.
        /// </summary>
        Remove = 4
    }
}
