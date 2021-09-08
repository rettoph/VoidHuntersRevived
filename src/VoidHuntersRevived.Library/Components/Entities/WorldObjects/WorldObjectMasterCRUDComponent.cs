using Guppy.Attributes;
using Guppy.DependencyInjection;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Network.Extensions.DependencyInjection;
using Guppy.Network.Services;
using Guppy.Network.Utilities;
using Guppy.Utilities;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Interfaces;

namespace VoidHuntersRevived.Library.Components.Entities.WorldObjects
{
    [NetworkAuthorizationRequired(NetworkAuthorization.Master)]
    internal sealed class WorldObjectMasterCRUDComponent : WorldObjectBaseCRUDComponent
    {
        #region Private Fields
        private Broadcast _broadcast;
        #endregion

        #region Lifecycle Methods
        protected override void InitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.InitializeRemote(provider, networkAuthorization);

            _broadcast = provider.GetBroadcast(Constants.Messages.WorldObject.WorldInfoPing);

            this.Entity.OnWorldInfoDirtyChanged += this.HandleWorldInfoChangeDetected;

            this.Entity.Messages[Guppy.Network.Constants.Messages.NetworkEntity.Create].OnWrite += this.WriteCreateMessage;
            this.Entity.Messages[Constants.Messages.WorldObject.WorldInfoPing].OnWrite += this.WriteWorldInfoPingMessage;
        }

        protected override void ReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.ReleaseRemote(networkAuthorization);

            this.Entity.Messages[Constants.Messages.WorldObject.WorldInfoPing].OnWrite -= this.WriteWorldInfoPingMessage;
            this.Entity.Messages[Guppy.Network.Constants.Messages.NetworkEntity.Create].OnWrite -= this.WriteCreateMessage;

            this.Entity.OnWorldInfoDirtyChanged -= this.HandleWorldInfoChangeDetected;

            _broadcast = default;
        }
        #endregion

        #region Network Methods
        private void WriteCreateMessage(MessageTypeManager sender, NetOutgoingMessage im)
        {
            this.Entity.Messages[Constants.Messages.WorldObject.WorldInfoPing].TryWrite(im);
        }

        private void WriteWorldInfoPingMessage(MessageTypeManager sender, NetOutgoingMessage om)
        {
            this.Entity.WriteWorldInfo(om);
        }
        #endregion

        #region Event Handlers
        private void CleanWorldObjectWorldInfo(NetOutgoingMessage om)
        {
            this.Entity.WorldInfoDirty = false;
        }

        private void HandleWorldInfoChangeDetected(IWorldObject sender, Boolean dirty)
        {
            if (dirty)
                _broadcast.Enqueue(this.Entity, this.CleanWorldObjectWorldInfo);
        }
        #endregion
    }
}
