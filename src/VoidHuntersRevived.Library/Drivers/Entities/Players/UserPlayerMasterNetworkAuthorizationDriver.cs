using Guppy.DependencyInjection;
using Guppy.Extensions.System;
using Guppy.Lists;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Drivers.Entities.Players
{
    internal sealed class UserPlayerMasterNetworkAuthorizationDriver : MasterNetworkAuthorizationDriver<UserPlayer>
    {
        #region Private Fields
        private EntityList _entities;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(UserPlayer driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            provider.Service(out _entities);
            
            this.driven.Actions.Set("update:ship:target:request", this.HandleUpdateShipTargetRequestMessage);
            this.driven.Actions.Set("ship:tractor-beam:action:request", this.HandleShipTractorBeamActionRequestMessage);
        }
        #endregion

        #region Message Handlers
        private void HandleUpdateShipTargetRequestMessage(NetIncomingMessage im)
            => this.driven.Ship.ReadTarget(im);

        private void HandleShipTractorBeamActionRequestMessage(NetIncomingMessage im)
        {
            var response = this.driven.Ship.TractorBeam.TryAction(
                action: new TractorBeam.Action(
                    type: (TractorBeam.ActionType)im.ReadByte(),
                    target: im.ReadEntity<ShipPart>(_entities).Then(sp =>
                    {
                        if (im.ReadBoolean())
                            sp.SetTransformIgnoreContacts(
                                position: im.ReadVector2(),
                                angle: im.ReadSingle());
                    })));
        }
        #endregion
    }
}
