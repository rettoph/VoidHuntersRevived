using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network;
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
using VoidHuntersRevived.Library.Messages.Requests;

namespace VoidHuntersRevived.Library.Components.Ships
{
    [HostTypeRequired(HostType.Remote)]
    [NetworkAuthorizationRequired(NetworkAuthorization.Slave)]
    internal sealed class DirectionRemoteSlaveComponent : ReferenceComponent<Ship, DirectionComponent>,
        IDataProcessor<ShipDirectionChangedMessage>
    {
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.Entity.Messages.RegisterProcessor<ShipDirectionChangedMessage>(this);
        }

        protected override void Uninitialize()
        {
            base.Uninitialize();

            this.Entity.Messages.DeregisterProcessor<ShipDirectionChangedMessage>(this);
        }

        bool IDataProcessor<ShipDirectionChangedMessage>.Process(ShipDirectionChangedMessage data)
        {
            this.Reference.EnqueueRequest(data.Direction, data.State, HostType.Remote);

            return true;
        }
    }
}
