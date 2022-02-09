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
    [NetworkAuthorizationRequired(NetworkAuthorization.Master)]
    internal sealed class TargetRemoteMasterComponent : ReferenceComponent<Ship, TargetComponent>,
        IDataFactory<ShipTargetMessage>
    {
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.Entity.Messages.RegisterPinger(
                this,
                Globals.Constants.Intervals.MinimumShipTargetPingInterval,
                Globals.Constants.Intervals.MaximumShipTargetPingInterval);
        }

        ShipTargetMessage IDataFactory<ShipTargetMessage>.Create()
        {
            return new ShipTargetMessage()
            {
                Value = this.Reference.Value
            };
        }
    }
}
