using Guppy.DependencyInjection;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.WorldObjects;

namespace VoidHuntersRevived.Library.Components.Entities.WorldObjects
{
    internal class WorldObjectBaseCRUDComponent : RemoteHostComponent<IWorldObject>
    {
        #region Lifecycle Methods
        protected override void InitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.InitializeRemote(provider, networkAuthorization);

            this.Entity.Messages.Add(
                messageType: Constants.Messages.WorldObject.WorldInfoPing,
                defaultContext: Guppy.Network.Constants.MessageContexts.InternalUnreliableDefault);
        }
        #endregion
    }
}
