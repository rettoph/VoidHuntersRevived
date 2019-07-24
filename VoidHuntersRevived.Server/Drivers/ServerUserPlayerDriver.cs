using Guppy.Implementations;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Server.Drivers
{
    public class ServerUserPlayerDriver : Driver
    {
        private UserPlayer _player;

        #region Constructors
        public ServerUserPlayerDriver(UserPlayer parent, IServiceProvider provider) : base(parent, provider)
        {
            _player = parent;
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            _player.AddActionHandler("set:direction", this.HandleSetDirectionAction);
            _player.AddActionHandler("tractor-beam:select", this.HandleTractorBeamSelectAction);
            _player.AddActionHandler("tractor-beam:release", this.HandleTractorBeamReleaseAction);
            _player.AddActionHandler("tractor-beam:set:offset", this.HandleTractorBeamSetOffsetAction);
        }
        #endregion

        #region Action Handlers
        private void HandleSetDirectionAction(NetIncomingMessage obj)
        {
            // Read any incoming direction update data
            _player.Ship.ReadDirectionData(obj);
        }

        private void HandleTractorBeamSelectAction(NetIncomingMessage obj)
        {
            // Read any incoming offset data
            _player.Ship.TractorBeam.ReadOffsetData(obj);
            // Read any incoming target data
            _player.Ship.TractorBeam.ReadSelectedData(obj);
        }

        private void HandleTractorBeamReleaseAction(NetIncomingMessage obj)
        {
            // Read any incoming offset data
            _player.Ship.TractorBeam.ReadOffsetData(obj);
            // Release the tractor beam
            _player.Ship.TractorBeam.TryRelease();
        }

        private void HandleTractorBeamSetOffsetAction(NetIncomingMessage obj)
        {
            // Read any incoming offset data
            _player.Ship.TractorBeam.ReadOffsetData(obj);
        }
        #endregion
    }
}
