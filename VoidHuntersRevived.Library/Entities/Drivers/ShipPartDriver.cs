using System;
using System.Collections.Generic;
using System.Text;
using Guppy;
using Guppy.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Entities.Drivers
{
    public abstract class ShipPartDriver : Driver
    {
        protected ShipPart parent { get; private set; }

        public ShipPartDriver(
            ShipPart parent,
            EntityConfiguration configuration,
            Scene scene,
            ILogger logger) : 
                base(
                    configuration,
                    scene, 
                    logger)
        {
            this.parent = parent;
        }
    }
}
