using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Threading.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Messages.Network;

namespace VoidHuntersRevived.Library.Components.Ships
{
    [HostTypeRequired(HostType.Remote)]
    [NetworkAuthorizationRequired(NetworkAuthorization.Slave)]
    internal sealed class TargetRemoteSlaveComponent : ReferenceComponent<Ship, TargetComponent>,
        IDataProcessor<ShipTargetMessage>
    {
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.Entity.Messages.RegisterProcessor<ShipTargetMessage>(this);
        }

        protected override void Uninitialize()
        {
            base.Uninitialize();

            this.Entity.Messages.DeregisterProcessor<ShipTargetMessage>(this);
        }

        bool IDataProcessor<ShipTargetMessage>.Process(ShipTargetMessage data)
        {
            this.Reference.SetValue(data.Value);

            return true;
        }
    }
}
