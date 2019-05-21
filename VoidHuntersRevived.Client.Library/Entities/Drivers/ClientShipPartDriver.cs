using System;
using System.Collections.Generic;
using System.Text;
using Guppy;
using Guppy.Collections;
using Guppy.Configurations;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Entities.Drivers;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Client.Library.Entities.Drivers
{
    public class ClientShipPartDriver : ShipPartDriver
    {
        public ClientShipPartDriver(
            ShipPart parent, 
            EntityConfiguration configuration, 
            Scene scene, 
            ILogger logger) : 
                base(
                    parent, 
                    configuration, 
                    scene, 
                    logger)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }
    }
}
