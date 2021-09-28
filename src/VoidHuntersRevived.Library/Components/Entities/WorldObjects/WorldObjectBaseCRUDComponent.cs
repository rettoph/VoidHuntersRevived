using Guppy.DependencyInjection;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Interfaces;

namespace VoidHuntersRevived.Library.Components.Entities.WorldObjects
{
    internal class WorldObjectBaseCRUDComponent : NetworkComponent<IWorldObject>
    {
        #region Lifecycle Methods
        protected override void PreInitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.PreInitializeRemote(provider, networkAuthorization);

            this.Entity.Messages.Add(
                messageType: Constants.Messages.WorldObject.WorldInfoPing,
                defaultContext: Constants.MessageContexts.WorldObject.WorldInfoPingMessageContext);
        }

        protected override void PostReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.PostReleaseRemote(networkAuthorization);

            this.Entity.Pipe = default;
        }
        #endregion
    }
}
