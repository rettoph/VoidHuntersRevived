using Guppy.DependencyInjection;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    internal sealed class ShipSlaveNetworkAuthorizationDriver : SlaveNetworkAuthorizationDriver<Ship>
    {
        #region Private Fields
        private Single _energyTarget;
        #endregion

        #region Lifecycle Methods
        protected override void InitializeRemote(Ship driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            this.driven.MessageHandlers[MessageType.Update].OnRead += this.ReadUpdate;

            this.driven.Ping.Set(VHR.Network.Pings.Ship.UpdateFiring, this.HandleUpdateFiringMessage);
            this.driven.Ping.Set(VHR.Network.Pings.Ship.UpdateBridge, this.HandleUpdateShipBridgeMessage);
            this.driven.Ping.Set(VHR.Network.Pings.Ship.UpdateDirection, this.HandleUpdateDirectionMessage);
            this.driven.Ping.Set(VHR.Network.Pings.Ship.TractorBeam.Action, this.HandleTractorBeamActionMessage);

            this.driven.OnPostUpdate += this.PostUpdate;
        }

        protected override void ReleaseRemote(Ship driven)
        {
            base.ReleaseRemote(driven);

            this.driven.MessageHandlers[MessageType.Update].OnRead -= this.ReadUpdate;

            this.driven.Ping.Remove(VHR.Network.Pings.Ship.UpdateFiring);
            this.driven.Ping.Remove(VHR.Network.Pings.Ship.UpdateBridge);
            this.driven.Ping.Remove(VHR.Network.Pings.Ship.UpdateDirection);
            this.driven.Ping.Remove(VHR.Network.Pings.Ship.TractorBeam.Action);

            this.driven.OnPostUpdate -= this.PostUpdate;
        }
        #endregion

        #region Frame Methods
        private void PostUpdate(GameTime gameTime)
        {
            this.driven.Energy = MathHelper.Lerp(this.driven.Energy, _energyTarget, VHR.Utilities.SlaveLerpPerSecond * (Single)gameTime.ElapsedGameTime.TotalSeconds);
        }
        #endregion

        #region Network Methods
        private void ReadUpdate(NetIncomingMessage im)
        {
            this.driven.ReadTarget(im);

            _energyTarget = im.ReadSingle();
            this.driven.Charging = im.ReadBoolean();
        }
        #endregion

        #region Message Handlers
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
