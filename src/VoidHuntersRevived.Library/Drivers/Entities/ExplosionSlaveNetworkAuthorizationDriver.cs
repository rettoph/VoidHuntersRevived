using Guppy.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    internal sealed class ExplosionSlaveNetworkAuthorizationDriver : SlaveNetworkAuthorizationDriver<Explosion>
    {
        #region Lifecycle Methods
        protected override void InitializeRemote(Explosion driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            this.driven.MessageHandlers[MessageType.Setup].OnRead += this.driven.ReadContext;
        }

        protected override void ReleaseRemote(Explosion driven)
        {
            base.ReleaseRemote(driven);

            this.driven.MessageHandlers[MessageType.Setup].OnRead -= this.driven.ReadContext;
        }
        #endregion
    }
}
