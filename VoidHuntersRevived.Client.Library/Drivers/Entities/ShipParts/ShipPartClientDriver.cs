using FarseerPhysics.Dynamics;
using Guppy;
using Guppy.Attributes;
using Microsoft.Xna.Framework;
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

        #region Lifecycle Methods
        protected override void PostInitialize()
        {
            base.PostInitialize();

            _shadow = _server[this.driven];

            this.driven.MaleConnectionNode.OnDetached += this.OnMaleConnectionNodeDetached;
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.driven.MaleConnectionNode.OnDetached -= this.OnMaleConnectionNodeDetached;
        }
        #endregion

        #region Event Handlers
        private void OnMaleConnectionNodeDetached(Object Sender, ConnectionNode node)
        {
            // Update the server shadow's Body's world position
            this.driven.SetWorldTransform(node.Parent.Root, _shadow);
        }
        #endregion
    }
}
