using Guppy.DependencyInjection;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using Guppy.Network.Extensions.Lidgren;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    internal sealed class WorldEntitySlaveNetworkAuthorizationDriver : SlaveNetworkAuthorizationDriver<WorldEntity>
    {
        #region Lifecycle Methods
        protected override void InitializeRemote(WorldEntity driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            this.driven.Actions.Set("update:size", this.driven.ReadSize);
        }

        protected override void ReleaseRemote(WorldEntity driven)
        {
            base.ReleaseRemote(driven);

            this.driven.Actions.Remove("update:size");
        }
        #endregion
    }
}
