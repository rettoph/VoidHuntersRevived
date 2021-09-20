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
    [NetworkAuthorizationRequired(NetworkAuthorization.Master)]
    internal sealed class ShipTargetingMasterCRUDComponent : ShipTargetingComponent
    {
        #region Private Fields
        private Broadcast _broadcast;

        private Boolean _dirty;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.PreInitializeRemote(provider, networkAuthorization);

            _broadcast = provider.GetBroadcast(Constants.Messages.Ship.TargetChanged);

            this.OnTargetChanged += this.HandleTargetChanged;

            this.Entity.Messages[Guppy.Network.Constants.Messages.NetworkEntity.Create].OnWrite += this.WriteCreateMessage;
            this.Entity.Messages[Constants.Messages.Ship.TargetChanged].OnWrite += this.WriteShipTargetChangedMessage;
        }

        protected override void PostReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.PostReleaseRemote(networkAuthorization);

            this.Entity.Messages[Constants.Messages.Ship.TargetChanged].OnWrite -= this.WriteShipTargetChangedMessage;
            this.Entity.Messages[Guppy.Network.Constants.Messages.NetworkEntity.Create].OnWrite -= this.WriteCreateMessage;

            this.OnTargetChanged -= this.HandleTargetChanged;

            _broadcast = default;
        }
        #endregion

        #region Network Methods
        private void WriteCreateMessage(MessageTypeManager sender, NetOutgoingMessage im)
        {
            this.Entity.Messages[Constants.Messages.Ship.TargetChanged].TryWrite(im);
        }

        private void WriteShipTargetChangedMessage(MessageTypeManager sender, NetOutgoingMessage om)
        {
            om.Write(this.Target);
        }
        #endregion

        #region Event Handlers
        private void CleanShipTarget(NetOutgoingMessage om)
        {
            _dirty = false;
        }

        private void HandleTargetChanged(Ship sender, Vector2 args)
        {
            if (_dirty)
                return;

            _dirty = true;
            _broadcast.Enqueue(this.Entity, this.CleanShipTarget);
        }
        #endregion
    }
}
