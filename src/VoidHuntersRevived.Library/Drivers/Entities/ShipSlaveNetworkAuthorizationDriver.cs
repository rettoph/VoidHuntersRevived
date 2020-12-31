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

            this.driven.Actions.Set("update:ship:target", this.HandleUpdateTargetMessage);
            this.driven.Actions.Set("update:ship:firing", this.HandleUpdateFiringMessage);
            this.driven.Actions.Set("update:ship:direction", this.HandleUpdateDirectionMessage);
            this.driven.Actions.Set("ship:tractor-beam:action", this.HandleTractorBeamActionMessage);
            this.driven.Actions.Set("update:ship:bridge", this.HandleUpdateShipBridgeMessage);
        }

        protected override void ReleaseRemote(Ship driven)
        {
            base.ReleaseRemote(driven);

            this.driven.Actions.Remove("update:ship:target");
            this.driven.Actions.Remove("update:ship:firing");
            this.driven.Actions.Remove("update:ship:direction");
            this.driven.Actions.Remove("ship:tractor-beam:action");
            this.driven.Actions.Remove("update:ship:bridge");
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
