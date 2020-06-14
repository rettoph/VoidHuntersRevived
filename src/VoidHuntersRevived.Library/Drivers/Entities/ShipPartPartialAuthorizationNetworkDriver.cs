using Guppy.Collections;
using Guppy.DependencyInjection;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    internal sealed class ShipPartPartialAuthorizationNetworkDriver : BaseAuthorizationDriver<ShipPart>
    {
        #region Private Fields
        private EntityCollection _entities;
        #endregion

        #region Lifecycle Methods
        protected override void ConfigurePartial(ServiceProvider provider)
        {
            base.ConfigurePartial(provider);

            provider.Service(out _entities);

            this.driven.Actions.Set("male-connection-node", this.ReadMaleConnectionNode);
        }

        protected override void DisposePartial()
        {
            base.DisposePartial();

            this.driven.Actions.Remove("male-connection-node");
        }
        #endregion

        #region Network Methods
        public void ReadMaleConnectionNode(NetIncomingMessage im)
        {
            if (im.ReadBoolean())
            { // Attempt to create the attachment (if any)
                this.driven.MaleConnectionNode.TryAttach(_entities.GetById<ShipPart>(im.ReadGuid()).FemaleConnectionNodes[im.ReadInt32()]);
            }
        }
        #endregion
    }
}
