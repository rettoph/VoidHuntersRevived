using Guppy.DependencyInjection;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using Guppy.Network.Extensions.Lidgren;
using Microsoft.Xna.Framework;
using System.Net.NetworkInformation;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    internal sealed class WorldEntityMasterNetworkAuthorizationDriver : MasterNetworkAuthorizationDriver<WorldEntity>
    {
        #region Lifecycle Methods
        protected override void Initialize(WorldEntity driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            this.driven.OnSizeChanged += this.HandleSizeChanged;
        }

        protected override void Release(WorldEntity driven)
        {
            base.Release(driven);

            this.driven.OnSizeChanged -= this.HandleSizeChanged;
        }
        #endregion

        #region Event Handlers
        private void HandleSizeChanged(WorldEntity sender, Vector2 arg)
            => this.driven.Actions.Create(NetDeliveryMethod.ReliableUnordered, 2).Write("update:size", m =>
            {
                m.Write(this.driven.Size);
            });
        #endregion
    }
}
