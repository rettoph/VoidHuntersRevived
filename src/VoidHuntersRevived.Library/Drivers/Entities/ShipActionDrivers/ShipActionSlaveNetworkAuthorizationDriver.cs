using Guppy.DependencyInjection;
using Guppy.Utilities;
using Lidgren.Network;
using log4net;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Drivers.Entities.ShipActionDrivers
{
    internal sealed class ShipActionSlaveNetworkAuthorizationDriver : SlaveNetworkAuthorizationDriver<Ship>
    {
        #region Private Fields
        private Synchronizer _synchronizer;
        private ILog _logger;
        #endregion

        #region Lifecycle Methods
        protected override void InitializeRemote(Ship driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            provider.Service(out _synchronizer);
            provider.Service(out _logger);

            this.driven.Ping.Set(VHR.Network.Pings.Ship.Action, this.ReadActionPing);
        }

        protected override void ReleaseRemote(Ship driven)
        {
            base.ReleaseRemote(driven);

            this.driven.Ping.Remove(VHR.Network.Pings.Ship.Action);
        }
        #endregion

        #region Network Methods
        private void ReadActionPing(NetIncomingMessage im)
        {
            var actionId = im.ReadUInt32();
            var data = im.ReadByte();

            _synchronizer.Enqueue(gt =>
            {
                this.driven?.Actions.TryInvoke(actionId, gt, data);
            });
        }
        #endregion
    }
}
