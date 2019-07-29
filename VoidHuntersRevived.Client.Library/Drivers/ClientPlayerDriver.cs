using Guppy.Collections;
using Guppy.Implementations;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Drivers;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Client.Library.Drivers
{
    public class ClientPlayerDriver : Driver
    {
        private Player _player;
        private EntityCollection _entities;

        #region Constructors
        public ClientPlayerDriver(Player parent, EntityCollection entities, IServiceProvider provider) : base(parent, provider)
        {
            _player = parent;
            _entities = entities;
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            _player.Actions.AddHandler("set:ship", this.HandleSetShipAction);
            _player.Actions.AddHandler("set:direction", this.HandleSetShipDirection);
            _player.Actions.AddHandler("tractor-beam:select", this.HandleTractorBeamSelectAction);
            _player.Actions.AddHandler("tractor-beam:release", this.HandleTractorBeamReleaseAction);
            _player.Actions.AddHandler("tractor-beam:set:offset", this.HandleTractorBeamSetOffsetAction);
            _player.Actions.AddHandler("tractor-beam:decline:select", this.HandleTractorBeamDeclineSelectAction);
            _player.Actions.AddHandler("tractor-beam:decline:attach", this.HandleTractorBeamDeclineAttachAction);
        }
        #endregion

        #region Action Handlers
        private void HandleSetShipAction(Object sender, NetIncomingMessage obj)
        {
            _player.ReadShipData(obj);
        }

        private void HandleSetShipDirection(Object sender, NetIncomingMessage obj)
        {
            // Read any incoming ship direction data
            _player.Ship.ReadDirectionData(obj);
        }

        private void HandleTractorBeamSelectAction(Object sender, NetIncomingMessage obj)
        {
            // Read any incoming offset data
            _player.Ship.TractorBeam.ReadOffsetData(obj);
            // Read any incoming target data
            _player.Ship.TractorBeam.ReadSelectedData(obj);
        }

        private void HandleTractorBeamReleaseAction(Object sender, NetIncomingMessage obj)
        {
            // Read any incoming offset data
            _player.Ship.TractorBeam.ReadOffsetData(obj);
            // Release the tractor beam
            _player.Ship.TractorBeam.TryRelease();
        }

        private void HandleTractorBeamSetOffsetAction(Object sender, NetIncomingMessage obj)
        {
            // Read any incoming offset data
            _player.Ship.TractorBeam.ReadOffsetData(obj);
        }

        private void HandleTractorBeamDeclineSelectAction(Object sender, NetIncomingMessage obj)
        {
            this.logger.LogWarning($"TractorBeam select request declined...");

            // Immediately release the tractor beam
            _player.Ship.TractorBeam.TryRelease();
        }

        private void HandleTractorBeamDeclineAttachAction(Object sender, NetIncomingMessage obj)
        {
            this.logger.LogWarning($"TractorBeam attach request declined...");

            var targetId = obj.ReadInt32();
            // Immediately attempt to detach the ship part recieved in the message
            obj.ReadEntity<ShipPart>(_entities)
                .FemaleConnectionNodes
                .First(f => f.Id == targetId)?
                .Target
                .Parent
                .TryDetatchFrom();
        }
        #endregion
    }
}
