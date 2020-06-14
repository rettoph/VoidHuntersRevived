using Guppy.DependencyInjection;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Extensions.Farseer;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    internal sealed class WorldEntityPartialAuthorizationNetworkDriver : BaseAuthorizationDriver<WorldEntity>
    {
        #region Lifecycle Methods
        protected override void ConfigurePartial(ServiceProvider provider)
        {
            base.ConfigureFull(provider);

            this.driven.Actions.Set("update:size", this.ReadSize);
        }

        protected override void DisposePartial()
        {
            base.DisposePartial();

            this.driven.Actions.Remove("update:size");
        }
        #endregion

        #region Network Methods
        private void ReadSize(NetIncomingMessage im)
            => this.driven.Size = im.ReadVector2();
        #endregion
    }
}
