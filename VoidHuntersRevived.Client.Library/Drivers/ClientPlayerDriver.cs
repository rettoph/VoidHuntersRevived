﻿using Guppy.Collections;
using Guppy.Implementations;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            _player.AddActionHandler("set:ship", this.HandleSetShipAction);
            _player.AddActionHandler("set:direction", this.HandleSetShipDirection);
            _player.AddActionHandler("tractor-beam:select", this.HandleTractorBeamSelectAction);
            _player.AddActionHandler("tractor-beam:release", this.HandleTractorBeamReleaseAction);
            _player.AddActionHandler("tractor-beam:set:offset", this.HandleTractorBeamSetOffsetAction);
            _player.AddActionHandler("tractor-beam:decline:select", this.HandleTractorBeamDeclineSelectAction);
            _player.AddActionHandler("tractor-beam:decline:attach", this.HandleTractorBeamDeclineAttachAction);
        }
        #endregion

        #region Action Handlers
        private void HandleSetShipAction(NetIncomingMessage obj)
        {
            _player.ReadShipData(obj);
        }

        private void HandleSetShipDirection(NetIncomingMessage obj)
        {
            // Read any incoming ship direction data
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

        private void HandleTractorBeamDeclineSelectAction(NetIncomingMessage obj)
        {
            this.logger.LogWarning($"TractorBeam select request declined...");

            // Immediately release the tractor beam
            _player.Ship.TractorBeam.TryRelease();
        }

        private void HandleTractorBeamDeclineAttachAction(NetIncomingMessage obj)
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
