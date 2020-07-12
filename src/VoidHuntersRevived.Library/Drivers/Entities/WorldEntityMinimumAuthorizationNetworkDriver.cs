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
    internal sealed class WorldEntityMinimumAuthorizationNetworkDriver : BaseAuthorizationDriver<WorldEntity>
    {
        #region Lifecycle Methods
        protected override void ConfigureMinimum(ServiceProvider provider)
        {
            base.ConfigureMinimum(provider);

            this.driven.Actions.Set("update:size", this.ReadSize);
        }

        protected override void DisposeMinimum()
        {
            base.DisposeMinimum();

            this.driven.Actions.Remove("update:size");
        }
        #endregion

        #region Network Methods
        private void ReadSize(NetIncomingMessage im)
            => this.driven.Size = im.ReadVector2();
        #endregion
    }
}
