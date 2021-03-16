using Guppy.DependencyInjection;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts.Special;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Entities.ShipParts.SpellParts;

namespace VoidHuntersRevived.Library.Drivers.Entities.ShipParts.SpellParts
{
    internal sealed class SpellPartMasterAuthorizationNetworkDriver : MasterNetworkAuthorizationDriver<SpellPart>
    {
        #region Lifecycle Methods
        protected override void InitializeRemote(SpellPart driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            this.driven.OnPreCast += this.HandleCast;
        }

        protected override void ReleaseRemote(SpellPart driven)
        {
            base.ReleaseRemote(driven);

            this.driven.OnPreCast -= this.HandleCast;
        }
        #endregion

        #region Event Handlers
        private void HandleCast(SpellPart sender, GameTime args)
            => this.driven.Ping.Create(NetDeliveryMethod.ReliableUnordered, 0).Write(VHR.Network.Pings.SpellCasterPart.Cast, om =>
            {
                //
            });
        #endregion
    }
}
