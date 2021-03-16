using Guppy.DependencyInjection;
using Guppy.Utilities;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts.Special;
using VoidHuntersRevived.Library.Entities.ShipParts.SpellParts;

namespace VoidHuntersRevived.Library.Drivers.Entities.ShipParts.SpellParts
{
    internal sealed class SpellPartSlaveAuthorizationNetworkDriver : SlaveNetworkAuthorizationDriver<SpellPart>
    {
        #region Private Fields
        private Synchronizer _synchronizer;
        #endregion

        #region Lifecycle Methods
        protected override void InitializeRemote(SpellPart driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            provider.Service(out _synchronizer);

            this.driven.Ping.Set(VHR.Network.Pings.SpellCasterPart.Cast, this.ReadCast);
        }

        protected override void ReleaseRemote(SpellPart driven)
        {
            base.ReleaseRemote(driven);

            _synchronizer = null;

            this.driven.Ping.Remove(VHR.Network.Pings.SpellCasterPart.Cast);
        }
        #endregion

        #region Event Handlers
        private void ReadCast(NetIncomingMessage obj)
            => _synchronizer.Enqueue(gt => this.driven?.TryCast(gt, true));
        #endregion
    }
}
