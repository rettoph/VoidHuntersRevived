using Guppy.DependencyInjection;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Network.Utilities;
using Guppy.Utilities;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Globals.Constants;

namespace VoidHuntersRevived.Library.Components.Entities.WorldObjects
{
    [NetworkAuthorizationRequired(NetworkAuthorization.Slave)]
    internal sealed class WorldObjectSlaveCRUDComponent : WorldObjectBaseCRUDComponent
    {
        #region Lifecycle Methods
        protected override void PreInitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.PreInitializeRemote(provider, networkAuthorization);

            this.Entity.Messages[Guppy.Network.Constants.Messages.NetworkEntity.Create].OnRead += this.ReadCreateMessage;
            this.Entity.Messages[Messages.WorldObject.WorldInfoPing].OnRead += this.ReadWorldInfoPingMessage;
        }

        protected override void PostReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.PostReleaseRemote(networkAuthorization);

            this.Entity.Messages[Guppy.Network.Constants.Messages.NetworkEntity.Create].OnRead += this.ReadCreateMessage;
            this.Entity.Messages[Messages.WorldObject.WorldInfoPing].OnRead -= this.ReadWorldInfoPingMessage;
        }
        #endregion

        #region Network Methods
        private void ReadCreateMessage(MessageTypeManager sender, NetIncomingMessage im)
        {
            this.Entity.Messages[Messages.WorldObject.WorldInfoPing].TryRead(im);
        }

        private void ReadWorldInfoPingMessage(MessageTypeManager sender, NetIncomingMessage im)
        {
            this.Entity.ReadWorldInfo(im);
        }
        #endregion
    }
}
