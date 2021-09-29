using Guppy.DependencyInjection;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Structs;
using VoidHuntersRevived.Library.Extensions.Lidgren;
using VoidHuntersRevived.Library.Enums;
using Guppy.Network.Extensions.Lidgren;

namespace VoidHuntersRevived.Library.Components.Entities.Ships
{
    [NetworkAuthorizationRequired(NetworkAuthorization.Master)]
    internal sealed class ShipTractorBeamMasterCRUDComponent : ShipTractorBeamComponent
    {
        #region Lifecycle Methods
        protected override void InitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.InitializeRemote(provider, networkAuthorization);

            this.OnAction += this.HandleShipTractorBeamAction;
        }

        protected override void ReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.ReleaseRemote(networkAuthorization);

            this.OnAction -= this.HandleShipTractorBeamAction;
        }
        #endregion

        #region Event Handlers
        private void HandleShipTractorBeamAction(ShipTractorBeamComponent sender, TractorBeamAction args)
        {
            this.Entity.Messages[Constants.Messages.Ship.TractorBeamAction].Create(om =>
            {
                om.Write(this.Entity.Components.Get<ShipTargetingComponent>().Target);
                om.Write(args, this.shipParts, ShipPartSerializationFlags.None);
            }, this.Entity.Pipe);
        }
        #endregion
    }
}
