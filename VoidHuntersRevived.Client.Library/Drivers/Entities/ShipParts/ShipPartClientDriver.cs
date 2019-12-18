using FarseerPhysics.Dynamics;
using Guppy;
using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities.ShipParts
{
    [IsDriver(typeof(ShipPart))]
    public class ShipPartClientDriver : Driver<ShipPart>
    {
        private ServerShadow _server;
        private Body _shadow;

        public ShipPartClientDriver(ServerShadow server, ShipPart driven) : base(driven)
        {
            _server = server;
        }

        protected override void PostInitialize()
        {
            base.PostInitialize();

            _shadow = _server[this.driven];

            // TODO: deregister event handler somewhere
            this.driven.MaleConnectionNode.OnDetached += (s, n) =>
            {
                // Update the server shadow's Body's world position
                this.driven.SetWorldTransform(n.Parent.Root, _shadow);
            };
        }
    }
}
