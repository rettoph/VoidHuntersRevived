using Guppy.Collections;
using Guppy.DependencyInjection;
using Guppy.Network;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Guppy.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Library.Drivers.Entities.Players
{
    internal sealed class UserPlayerFullAuthorizationNetworkDriver : BaseAuthorizationDriver<UserPlayer>
    {
        #region Private Fields
        private EntityCollection _entities;
        #endregion

        #region Lifecycle Methods
        protected override void ConfigureFull(ServiceProvider provider)
        {
            base.ConfigureFull(provider);

            provider.Service(out _entities);

            this.driven.Actions.Add("update:ship:direction:request", this.HandleUpdateShipDirectionRequest);
            this.driven.Actions.Add("update:ship:target:request", this.HandleUpdateShipTargetRequest);
            this.driven.Actions.Add("ship:tractor-beam:action:request", this.HandleShipTractorBeamActionRequest);

            this.driven.OnUserChanged += this.HandleUserChanged;
            this.driven.OnWrite += this.WriteUser;
        }

        protected override void DisposeFull()
        {
            base.DisposeFull();

            this.driven.Actions.Remove("update:ship:direction:request");
            this.driven.Actions.Remove("update:ship:target:request");
            this.driven.Actions.Remove("ship:tractor-beam:action:request");

            this.driven.OnUserChanged -= this.HandleUserChanged;
            this.driven.OnWrite -= this.WriteUser;
        }
        #endregion

        #region Network Methods
        private void WriteUser(NetOutgoingMessage om)
        {
            om.Write("update:user", m =>
            {
                if (m.WriteExists(this.driven.User))
                    m.Write(this.driven.User.Id);
            });
        }
        #endregion

        #region Network Handlers
        private void HandleUpdateShipDirectionRequest(NetIncomingMessage im)
            => this.driven.Ship.TrySetDirection((Ship.Direction)im.ReadByte(), im.ReadBoolean());

        private void HandleUpdateShipTargetRequest(NetIncomingMessage im)
            => this.driven.Ship.Target = im.ReadVector2();

        private void HandleShipTractorBeamActionRequest(NetIncomingMessage im)
            => this.driven.Ship.TractorBeam.TryAction(new TractorBeam.Action((TractorBeam.ActionType)im.ReadByte(), im.ReadEntity<ShipPart>(_entities)));
        #endregion

        #region Event Handlers
        private void HandleUserChanged(UserPlayer sender, User arg)
            => this.WriteUser(this.driven.Actions.Create(NetDeliveryMethod.ReliableUnordered, 5));
        #endregion
    }
}
