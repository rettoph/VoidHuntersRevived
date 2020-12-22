using Guppy.DependencyInjection;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Drivers.Entities.ShipParts
{
    internal sealed class ShipPartSlaveNetworkAuthorizationDriver : SlaveNetworkAuthorizationDriver<ShipPart>
    {
        #region Lifecycle Methods
        protected override void Initialize(ShipPart driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            this.driven.MessageHandlers[MessageType.Setup].OnRead += this.driven.ReadMaleConnectionNode;
            this.driven.MessageHandlers[MessageType.Setup].OnRead += this.driven.ReadHealth;
            this.driven.MessageHandlers[MessageType.Update].OnRead += this.driven.ReadHealth;

            this.driven.Actions.Set("update:health", this.driven.ReadHealth);
        }

        protected override void Release(ShipPart driven)
        {
            base.Release(driven);

            this.driven.MessageHandlers[MessageType.Setup].OnRead -= this.driven.ReadMaleConnectionNode;
            this.driven.MessageHandlers[MessageType.Setup].OnRead -= this.driven.ReadHealth;
            this.driven.MessageHandlers[MessageType.Update].OnRead -= this.driven.ReadHealth;

            this.driven.Actions.Remove("update:health");
        }
        #endregion
    }
}
