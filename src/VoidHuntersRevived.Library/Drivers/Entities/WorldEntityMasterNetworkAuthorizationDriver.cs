using Guppy.DependencyInjection;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using Guppy.Network.Extensions.Lidgren;
using Microsoft.Xna.Framework;
using System.Net.NetworkInformation;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    internal sealed class WorldEntityMasterNetworkAuthorizationDriver : MasterNetworkAuthorizationDriver<WorldEntity>
    {
        #region Lifecycle Methods
        protected override void InitializeRemote(WorldEntity driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            this.driven.OnSizeChanged += this.HandleSizeChanged;
        }

        protected override void ReleaseRemote(WorldEntity driven)
        {
            base.ReleaseRemote(driven);

            this.driven.OnSizeChanged -= this.HandleSizeChanged;
        }
        #endregion

        #region Event Handlers
        private void HandleSizeChanged(WorldEntity sender, Vector2 arg)
            => this.driven.Actions.Create(NetDeliveryMethod.ReliableOrdered, 2).Write(VHR.MessageTypes.World.UpdateSize, m =>
            {
                m.Write(this.driven.Size);
            });
        #endregion
    }
}
