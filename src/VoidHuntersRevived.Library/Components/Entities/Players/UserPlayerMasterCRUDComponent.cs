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
using VoidHuntersRevived.Library.Components.Entities.Ships;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Extensions.Lidgren;
using VoidHuntersRevived.Library.Structs;

namespace VoidHuntersRevived.Library.Components.Entities.Players
{
    [NetworkAuthorizationRequired(NetworkAuthorization.Master)]
    internal sealed class UserPlayerMasterCRUDComponent : UserPlayerBaseCRUDComponent
    {
        #region Private Fields
        private ShipPartService _shipParts;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.PreInitializeRemote(provider, networkAuthorization);

            provider.Service(out _shipParts);

            this.Entity.Messages[Guppy.Network.Constants.Messages.NetworkEntity.Create].OnWrite += this.WriteCreateMessage;
            this.Entity.Messages[Constants.Messages.UserPlayer.RequestDirectionChanged].OnRead += this.ReadRequestDirectionChangedMessage;
            this.Entity.Messages[Constants.Messages.UserPlayer.RequestTractorBeamAction].OnRead += this.ReadRequestTractorBeamActionMessage;
        }

        protected override void PostReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.PostReleaseRemote(networkAuthorization);

            this.Entity.Messages[Guppy.Network.Constants.Messages.NetworkEntity.Create].OnWrite -= this.WriteCreateMessage;
        }
        #endregion

        #region Network Methods
        private void WriteCreateMessage(MessageTypeManager sender, NetOutgoingMessage om)
        {
            om.Write(this.Entity.User.Id);
        }

        private void ReadRequestDirectionChangedMessage(MessageTypeManager sender, NetIncomingMessage im)
        {
            this.Entity.Ship?.Components.Get<ShipDirectionComponent>().TrySetDirection(
                direction: im.ReadEnum<Direction>(),
                value: im.ReadBoolean());
        }

        private void ReadRequestTractorBeamActionMessage(MessageTypeManager sender, NetIncomingMessage im)
        {
            this.Entity.Ship?.Components.Get<ShipTractorBeamComponent>().TryAction(
                action: im.ReadTractorBeamAction(_shipParts, ShipPartSerializationFlags.None));
        }
        #endregion
    }
}
