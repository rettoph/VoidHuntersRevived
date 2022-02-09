using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Network.Utilities;
using Guppy.Threading.Interfaces;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Globals.Constants;
using VoidHuntersRevived.Library.Messages.Network.Packets;

namespace VoidHuntersRevived.Library.Components.WorldObjects
{
    [NetworkAuthorizationRequired(NetworkAuthorization.Slave)]
    internal sealed class WorldObjectSlaveCRUDComponent : WorldObjectBaseCRUDComponent,
        IDataProcessor<WorldObjectPositionPacket>
    {
        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Entity.Messages.RegisterProcessor<WorldObjectPositionPacket>(this);
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

            this.Entity.Messages.DeregisterProcessor<WorldObjectPositionPacket>(this);
        }
        #endregion

        #region Network Methods
        Boolean IDataProcessor<WorldObjectPositionPacket>.Process(WorldObjectPositionPacket message)
        {
            this.Entity.SetTransform(message.Position, message.Rotation);

            return true;
        }
        #endregion
    }
}
