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
        protected override void InitializeRemote(ShipPart driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            this.driven.Ping.Set(VHR.Network.Pings.ShipPart.UpdateHealth, this.driven.ReadHealth);

            this.driven.MessageHandlers[MessageType.Setup].OnRead += this.driven.ReadMaleConnectionNode;
            this.driven.MessageHandlers[MessageType.Setup].OnRead += this.driven.ReadHealth;
            this.driven.MessageHandlers[MessageType.Update].OnRead += this.driven.ReadHealth;
        }

        protected override void ReleaseRemote(ShipPart driven)
        {
            base.ReleaseRemote(driven);

            this.driven.Ping.Remove(VHR.Network.Pings.ShipPart.UpdateHealth);

            this.driven.MessageHandlers[MessageType.Setup].OnRead -= this.driven.ReadMaleConnectionNode;
            this.driven.MessageHandlers[MessageType.Setup].OnRead -= this.driven.ReadHealth;
            this.driven.MessageHandlers[MessageType.Update].OnRead -= this.driven.ReadHealth;
        }
        #endregion
    }
}
