using Guppy.DependencyInjection;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using Guppy.Network.Extensions.Lidgren;
using System.Linq;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    internal sealed class WorldEntityFullAuthorizationNetworkDriver : BaseAuthorizationDriver<WorldEntity>
    {
        #region Lifecycle Methods
        protected override void ConfigureFull(ServiceProvider provider)
        {
            base.ConfigureFull(provider);

            this.driven.OnWrite += this.WriteSize;
            this.driven.OnSizeChanged += this.HandleSizeChanged;
        }

        protected override void DisposeFull()
        {
            base.DisposeFull();

            this.driven.OnWrite -= this.WriteSize;
            this.driven.OnSizeChanged -= this.HandleSizeChanged;
        }
        #endregion


        #region Network Methods
        private void WriteSize(NetOutgoingMessage om)
        {
            om.Write("update:size", m =>
            {
                m.Write(this.driven.Size);
            });
        }
        #endregion

        #region Event Handlers
        private void HandleSizeChanged(WorldEntity sender, Vector2 arg)
            => this.WriteSize(this.driven.Actions.Create(NetDeliveryMethod.ReliableUnordered, 2));
        #endregion
    }
}
