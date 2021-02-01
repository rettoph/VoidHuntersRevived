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

            this.driven.Actions.Set(VHR.MessageTypes.Ship.UpdateTarget, this.HandleUpdateTargetMessage);
            this.driven.Actions.Set(VHR.MessageTypes.Ship.UpdateFiring, this.HandleUpdateFiringMessage);
            this.driven.Actions.Set(VHR.MessageTypes.Ship.UpdateBridge, this.HandleUpdateShipBridgeMessage);
            this.driven.Actions.Set(VHR.MessageTypes.Ship.UpdateDirection, this.HandleUpdateDirectionMessage);
            this.driven.Actions.Set(VHR.MessageTypes.Ship.TractorBeam.Action, this.HandleTractorBeamActionMessage);
        }

        protected override void ReleaseRemote(Ship driven)
        {
            base.ReleaseRemote(driven);

            this.driven.Actions.Remove(VHR.MessageTypes.Ship.UpdateTarget);
            this.driven.Actions.Remove(VHR.MessageTypes.Ship.UpdateFiring);
            this.driven.Actions.Remove(VHR.MessageTypes.Ship.UpdateBridge);
            this.driven.Actions.Remove(VHR.MessageTypes.Ship.UpdateDirection);
            this.driven.Actions.Remove(VHR.MessageTypes.Ship.TractorBeam.Action);
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
