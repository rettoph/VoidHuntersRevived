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
    internal class UserPlayerBaseCRUDComponent : RemoteHostComponent<UserPlayer>
    {
        #region Lifecycle Methods
        protected override void PreInitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.PreInitializeRemote(provider, networkAuthorization);

            this.Entity.Messages.Add(
                messageType: Constants.Messages.UserPlayer.RequestDirectionChanged,
                defaultContext: Guppy.Network.Constants.MessageContexts.InternalReliableSecondary);

            this.Entity.Messages.Add(
                messageType: Constants.Messages.UserPlayer.RequestTractorBeamAction,
                defaultContext: Guppy.Network.Constants.MessageContexts.InternalReliableSecondary);
        }
        #endregion
    }
}
