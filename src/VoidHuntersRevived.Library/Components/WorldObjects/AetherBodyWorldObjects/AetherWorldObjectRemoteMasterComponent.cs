using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Network.Messages;
using Guppy.Threading.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Messages.Network;
using VoidHuntersRevived.Library.Messages.Network.Packets;

namespace VoidHuntersRevived.Library.Components.WorldObjects
{
    [HostTypeRequired(HostType.Remote)]
    [NetworkAuthorizationRequired(NetworkAuthorization.Master)]
    internal sealed class AetherWorldObjectRemoteMasterComponent : Component<AetherBodyWorldObject>,
        IDataFactory<AetherBodyWorldObjectVelocityPacket>
    {
        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Entity.Messages.RegisterPacket<AetherBodyWorldObjectVelocityPacket, CreateNetworkEntityMessage>(this);
            this.Entity.Messages.RegisterPacket<AetherBodyWorldObjectVelocityPacket, WorldObjectPositionPing>(this);
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

            this.Entity.Messages.DeregisterPacket<AetherBodyWorldObjectVelocityPacket, CreateNetworkEntityMessage>(this);
            this.Entity.Messages.DeregisterPacket<AetherBodyWorldObjectVelocityPacket, WorldObjectPositionPing>(this);
        }
        #endregion

        #region Network Methods
        AetherBodyWorldObjectVelocityPacket IDataFactory<AetherBodyWorldObjectVelocityPacket>.Create()
        {
            return new AetherBodyWorldObjectVelocityPacket()
            {
                LinearVelocity = this.Entity.Body.LocalInstance.LinearVelocity,
                AngularVelocity = this.Entity.Body.LocalInstance.AngularVelocity
            };
        }
        #endregion
    }
}
