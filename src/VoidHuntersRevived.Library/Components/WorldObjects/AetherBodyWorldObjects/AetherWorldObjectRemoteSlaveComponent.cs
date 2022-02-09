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
using VoidHuntersRevived.Library.Messages.Network.Packets;

namespace VoidHuntersRevived.Library.Components.WorldObjects
{
    [HostTypeRequired(HostType.Remote)]
    [NetworkAuthorizationRequired(NetworkAuthorization.Slave)]
    internal sealed class AetherWorldObjectRemoteSlaveComponent : Component<AetherBodyWorldObject>,
        IDataProcessor<AetherBodyWorldObjectVelocityPacket>
    {
        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Entity.Messages.RegisterProcessor<AetherBodyWorldObjectVelocityPacket>(this);
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

            this.Entity.Messages.DeregisterProcessor<AetherBodyWorldObjectVelocityPacket>(this);
        }
        #endregion

        #region Network Methods
        Boolean IDataProcessor<AetherBodyWorldObjectVelocityPacket>.Process(AetherBodyWorldObjectVelocityPacket packet)
        {
            var body = this.Entity.Body.Instances[NetworkAuthorization.Master];
            body.LinearVelocity = packet.LinearVelocity;
            body.AngularVelocity = packet.AngularVelocity;

            return true;
        }
        #endregion
    }
}
