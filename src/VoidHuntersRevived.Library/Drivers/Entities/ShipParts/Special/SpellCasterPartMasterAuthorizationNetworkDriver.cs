using Guppy.DependencyInjection;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts.Special;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Drivers.Entities.ShipParts.Special
{
    internal sealed class SpellCasterPartMasterAuthorizationNetworkDriver : MasterNetworkAuthorizationDriver<SpellCasterPart>
    {
        #region Lifecycle Methods
        protected override void InitializeRemote(SpellCasterPart driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            this.driven.OnCast += this.HandleCast;
        }

        protected override void ReleaseRemote(SpellCasterPart driven)
        {
            base.ReleaseRemote(driven);

            this.driven.OnCast -= this.HandleCast;
        }
        #endregion

        #region Event Handlers
        private void HandleCast(SpellCasterPart sender, GameTime args)
            => this.driven.Ping.Create(NetDeliveryMethod.ReliableUnordered, 0).Write(VHR.Network.Pings.SpellCasterPart.Cast, om =>
            {
                //
            });
        #endregion
    }
}
