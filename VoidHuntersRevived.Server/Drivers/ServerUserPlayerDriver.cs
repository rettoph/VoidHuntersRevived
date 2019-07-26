using Guppy.Collections;
using Guppy.Implementations;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Extensions.Lidgren;

namespace VoidHuntersRevived.Server.Drivers
{
    public class ServerUserPlayerDriver : Driver
    {
        private UserPlayer _player;
        private EntityCollection _entities;

        #region Constructors
        public ServerUserPlayerDriver(UserPlayer parent, EntityCollection entities, IServiceProvider provider) : base(parent, provider)
        {
            _player = parent;
            _entities = entities;
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            _player.AddActionHandler("set:direction", this.HandleSetDirectionAction);
            _player.AddActionHandler("tractor-beam:select", this.HandleTractorBeamSelectAction);
            _player.AddActionHandler("tractor-beam:release", this.HandleTractorBeamReleaseAction);
            _player.AddActionHandler("tractor-beam:attach", this.HandleTractorBeamAttachAction);
            _player.AddActionHandler("tractor-beam:set:offset", this.HandleTractorBeamSetOffsetAction);
            
        }
        #endregion

        #region Helper Methods
        private Boolean ValidateSender(NetIncomingMessage im)
        {
            if (im.SenderConnection.RemoteUniqueIdentifier == _player.User.NetId)
                return true;

            im.SenderConnection.Disconnect("Invalid data recieved. Goodbye.");
            return false;
        }
        #endregion

        #region Action Handlers
        private void HandleSetDirectionAction(NetIncomingMessage obj)
        {
            if (this.ValidateSender(obj))
            {
                // Read any incoming direction update data
                _player.Ship.ReadDirectionData(obj);
            }
        }

        private void HandleTractorBeamSelectAction(NetIncomingMessage obj)
        {
            if (this.ValidateSender(obj))
            {
                // Read any incoming offset data
                _player.Ship.TractorBeam.ReadOffsetData(obj);
                // Read any incoming target data
                if (!_player.Ship.TractorBeam.ReadSelectedData(obj))
                { // If the server tractor beam was unable to select as requested...
                    var action = _player.CreateActionMessage("tractor-beam:decline:select", obj.SenderConnection);
                }
            }
        }

        private void HandleTractorBeamReleaseAction(NetIncomingMessage obj)
        {
            if (this.ValidateSender(obj))
            {
                // Read any incoming offset data
                _player.Ship.TractorBeam.ReadOffsetData(obj);
                // Read any incoming rotation data
                _player.Ship.TractorBeam.ReadRotationData(obj);
                // Release the tractor beam
                _player.Ship.TractorBeam.TryRelease();
            }
        }

        private void HandleTractorBeamAttachAction(NetIncomingMessage obj)
        {
            if (this.ValidateSender(obj))
            {
                // Read any incoming offset data
                _player.Ship.TractorBeam.ReadOffsetData(obj);
                // Pull the recieved female attachment node data...
                var target = obj.ReadFemaleConnectionNode(_entities);

                if (!_player.Ship.TractorBeam.TryAttatch(target))
                { // There was an error, we must send a decline message back the the connected client
                    var action = _player.CreateActionMessage("tractor-beam:decline:attach", obj.SenderConnection);
                    action.Write(target.Id);
                    action.Write(target.Parent);
                }

            }
        }

        private void HandleTractorBeamSetOffsetAction(NetIncomingMessage obj)
        {
            if (this.ValidateSender(obj))
            {
                // Read any incoming offset data
                _player.Ship.TractorBeam.ReadOffsetData(obj);
            }
        }
        #endregion
    }
}
