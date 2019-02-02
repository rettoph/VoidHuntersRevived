using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;

namespace VoidHuntersRevived.Library.Entities.Drivers
{
    /// <summary>
    /// Certain entities will contain a custom driver.
    /// Drivers can be used to interact with the entity
    /// directly, but are instantiated at runtime using
    /// a entity handle. This gives developers the ability
    /// to easily configure which drivers to use based on
    /// the server collection configuration.
    /// 
    /// For exampels of a driver in action, look at
    /// ShipPart.cs
    /// </summary>
    public abstract class Driver : Entity
    {
        public Driver(EntityInfo info, IGame game) : base(info, game)
        {
            // Drivers should always be updated by their owners.
            this.SetEnabled(false);
        }

        public abstract void Read(NetIncomingMessage im);

        public abstract void Write(NetOutgoingMessage om);
    }
}
