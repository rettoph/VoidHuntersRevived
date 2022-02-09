using Guppy.Attributes;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Network.Messages;
using Guppy.Network.Services;
using Guppy.Network.Utilities;
using Guppy.Threading.Interfaces;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Globals.Constants;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Library.Messages.Network;
using VoidHuntersRevived.Library.Messages.Network.Packets;

namespace VoidHuntersRevived.Library.Components.WorldObjects
{
    [NetworkAuthorizationRequired(NetworkAuthorization.Master)]
    internal sealed class WorldObjectMasterCRUDComponent : WorldObjectBaseCRUDComponent,
        IDataFactory<WorldObjectPositionPacket>
    {
        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Entity.Messages.RegisterPinger<WorldObjectPositionPing>(
                Globals.Constants.Intervals.MinimumWorldInfoPingInterval,
                Globals.Constants.Intervals.MaximumWorldInfoPingInterval);

            this.Entity.Messages.RegisterPacket<WorldObjectPositionPacket, CreateNetworkEntityMessage>(this);
            this.Entity.Messages.RegisterPacket<WorldObjectPositionPacket, WorldObjectPositionPing>(this);
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

            this.Entity.Messages.DeregisterPacket<WorldObjectPositionPacket, CreateNetworkEntityMessage>(this);
            this.Entity.Messages.DeregisterPacket<WorldObjectPositionPacket, WorldObjectPositionPing>(this);
        }
        #endregion

        #region Network Methods
        public WorldObjectPositionPacket Create()
        {
            return new WorldObjectPositionPacket()
            {
                Position = this.Entity.Position,
                Rotation = this.Entity.Rotation
            };
        }
        #endregion
    }
}
