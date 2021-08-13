using Guppy.DependencyInjection;
using Guppy.Enums;
using Guppy.Interfaces;
using Guppy.Network.Attributes;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Guppy.Network.Utilities;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Library.Components.Entities.Players
{
    [NetworkAuthorizationRequired(NetworkAuthorization.Master)]
    internal sealed class UserPlayerMasterCRUDComponent : RemoteHostComponent<UserPlayer>
    {
        #region Lifecycle Methods
        protected override void InitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.InitializeRemote(provider, networkAuthorization);

            this.Entity.Messages[Guppy.Network.Constants.Messages.NetworkEntity.Create].OnWrite += this.WriteCreateMessage;
        }

        protected override void ReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.ReleaseRemote(networkAuthorization);

            this.Entity.Messages[Guppy.Network.Constants.Messages.NetworkEntity.Create].OnWrite -= this.WriteCreateMessage;
        }
        #endregion

        #region Network Methods
        private void WriteCreateMessage(MessageTypeManager sender, NetOutgoingMessage om)
        {
            om.Write(this.Entity.User.Id);
        }
        #endregion
    }
}
