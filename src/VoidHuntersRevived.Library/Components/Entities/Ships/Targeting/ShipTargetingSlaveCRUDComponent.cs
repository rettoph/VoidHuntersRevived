using Guppy;
using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Network.Attributes;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Guppy.Network.Extensions.DependencyInjection;
using Guppy.Network.Utilities;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Ships;
using Guppy.Network.Extensions.Lidgren;

namespace VoidHuntersRevived.Library.Components.Entities.Ships
{
    [NetworkAuthorizationRequired(NetworkAuthorization.Slave)]
    internal sealed class ShipTargetingSlaveCrudComponent : ShipTargetingComponent
    {
        #region Lifecycle Methods
        protected override void PreInitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.PreInitializeRemote(provider, networkAuthorization);

            this.Entity.Messages[Guppy.Network.Constants.Messages.NetworkEntity.Create].OnRead += this.ReadCreateMessage;
            this.Entity.Messages[Constants.Messages.Ship.TargetChanged].OnRead += this.ReadShipTargetChangedMessage;
        }

        protected override void PostReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.PostReleaseRemote(networkAuthorization);

            this.Entity.Messages[Guppy.Network.Constants.Messages.NetworkEntity.Create].OnRead += this.ReadCreateMessage;
            this.Entity.Messages[Constants.Messages.Ship.TargetChanged].OnRead -= this.ReadShipTargetChangedMessage;
        }
        #endregion

        #region Network Methods
        private void ReadCreateMessage(MessageTypeManager sender, NetIncomingMessage im)
        {
            this.Entity.Messages[Constants.Messages.Ship.TargetChanged].TryRead(im);
        }

        private void ReadShipTargetChangedMessage(MessageTypeManager sender, NetIncomingMessage im)
        {
            this.Target = im.ReadVector2();
        }
        #endregion
    }
}
