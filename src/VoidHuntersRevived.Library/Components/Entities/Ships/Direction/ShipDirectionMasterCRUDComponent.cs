using Guppy;
using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.Network.Attributes;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Guppy.Network.Extensions.DependencyInjection;
using Guppy.Network.Utilities;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Ships;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Structs;

namespace VoidHuntersRevived.Library.Components.Entities.Ships
{
    [NetworkAuthorizationRequired(NetworkAuthorization.Master)]
    internal sealed class ShipDirectionMasterCRUDComponent : ShipDirectionComponent
    {
        protected override void InitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.InitializeRemote(provider, networkAuthorization);

            this.OnDirectionChanged += this.HandleShipDirectionChanged;
        }

        protected override void ReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.ReleaseRemote(networkAuthorization);

            this.OnDirectionChanged -= this.HandleShipDirectionChanged;
        }

        private void HandleShipDirectionChanged(ShipDirectionComponent sender, DirectionState args)
        {
            this.Entity.Messages[Constants.Messages.Ship.DirectionChanged].Create(om =>
            {
                om.Write(args.Direction);
                om.Write(args.State);
            }, this.Entity.Pipe);
        }
    }
}
