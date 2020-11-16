using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Extensions.Farseer;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    internal sealed class BodyEntitySlaveNetworkAuthorizationDriver : SlaveNetworkAuthorizationDriver<BodyEntity>
    {
        #region Lifecycle Methods
        protected override void Initialize(BodyEntity driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            this.driven.MessageHandlers[MessageType.Update].OnRead += this.driven.master.ReadPosition;
        }

        protected override void Release(BodyEntity driven)
        {
            base.Release(driven);

            this.driven.MessageHandlers[MessageType.Update].OnRead -= this.driven.master.ReadPosition;
        }
        #endregion
    }
}
