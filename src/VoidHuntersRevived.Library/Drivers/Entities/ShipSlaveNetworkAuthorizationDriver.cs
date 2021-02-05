using Guppy.DependencyInjection;
using Lidgren.Network;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    internal sealed class ShipSlaveNetworkAuthorizationDriver : SlaveNetworkAuthorizationDriver<Ship>
    {
        #region Lifecycle Methods
        protected override void InitializeRemote(Ship driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            this.driven.Ping.Set(VHR.Pings.Ship.UpdateTarget, this.HandleUpdateTargetMessage);
            this.driven.Ping.Set(VHR.Pings.Ship.UpdateFiring, this.HandleUpdateFiringMessage);
            this.driven.Ping.Set(VHR.Pings.Ship.UpdateBridge, this.HandleUpdateShipBridgeMessage);
            this.driven.Ping.Set(VHR.Pings.Ship.UpdateDirection, this.HandleUpdateDirectionMessage);
            this.driven.Ping.Set(VHR.Pings.Ship.TractorBeam.Action, this.HandleTractorBeamActionMessage);
        }

        protected override void ReleaseRemote(Ship driven)
        {
            base.ReleaseRemote(driven);

            this.driven.Ping.Remove(VHR.Pings.Ship.UpdateTarget);
            this.driven.Ping.Remove(VHR.Pings.Ship.UpdateFiring);
            this.driven.Ping.Remove(VHR.Pings.Ship.UpdateBridge);
            this.driven.Ping.Remove(VHR.Pings.Ship.UpdateDirection);
            this.driven.Ping.Remove(VHR.Pings.Ship.TractorBeam.Action);
        }
        #endregion

        #region Message Handlers
        private void HandleUpdateTargetMessage(NetIncomingMessage im)
            => this.driven.ReadTarget(im);

        private void HandleUpdateFiringMessage(NetIncomingMessage im)
            => this.driven.ReadFiring(im);

        private void HandleUpdateDirectionMessage(NetIncomingMessage im)
            => this.driven.ReadDirection(im);

        private void HandleTractorBeamActionMessage(NetIncomingMessage im)
            => this.driven.TractorBeam.ReadAction(im);

        private void HandleUpdateShipBridgeMessage(NetIncomingMessage im)
            => this.driven.ReadBridge(im);
        #endregion
    }
}
