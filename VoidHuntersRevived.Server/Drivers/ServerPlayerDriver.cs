using Guppy.Implementations;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.CustomEventArgs;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Server.Drivers
{
    public class ServerPlayerDriver : Driver
    {
        private Player _player;

        /// <summary>
        /// Stores the ship value (if any) that events are bound to currently
        /// This is stored so that we can properly remove events if the ship is
        /// changed.
        /// </summary>
        private Ship _eventTrackedShip;

        #region Constructors
        public ServerPlayerDriver(Player parent, IServiceProvider provider) : base(parent, provider)
        {
            _player = parent;
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            _player.Events.AddHandler("changed:ship", this.HandleShipChanged);
        }
        #endregion

        #region Event Handlers
        private void HandleShipChanged(Object arg)
        {
            var action = _player.CreateActionMessage("set:ship");
            _player.WriteShipData(action);

            // Remove any old ship events, and add any new ones
            if (_eventTrackedShip != null)
                this.RemoveShipEvents();
            if (arg != null)
                this.AddShipEvents(arg as Ship);
        }

        private void AddShipEvents(Ship ship)
        {
            _eventTrackedShip = ship;
            _eventTrackedShip.Events.AddHandler("changed:direction", this.HandleDirectionChanged);
            _eventTrackedShip.TractorBeam.Events.AddHandler("selected", this.HandleTractorBeamSelected);
            _eventTrackedShip.TractorBeam.Events.AddHandler("released", this.HandleTractorBeamRelease);
            _eventTrackedShip.TractorBeam.Events.AddHandler("changed:offset", this.HandleTractorBeamOffsetChanged);
        }

        private void RemoveShipEvents()
        {
            _eventTrackedShip.Events.RemoveHandler("changed:direction", this.HandleDirectionChanged);
            _eventTrackedShip.TractorBeam.Events.RemoveHandler("selected", this.HandleTractorBeamSelected);
            _eventTrackedShip.TractorBeam.Events.RemoveHandler("released", this.HandleTractorBeamRelease);
            _eventTrackedShip.TractorBeam.Events.RemoveHandler("changed:offset", this.HandleTractorBeamOffsetChanged);
        }

        /// <summary>
        /// When a server side ship's direction is changed, we
        /// must broadcast an update message to all connected
        /// clients.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleDirectionChanged(Object arg)
        {
            var action = _player.CreateActionMessage("set:direction");
            _player.Ship.WriteDirectionData(action, (Direction)arg);
        }

        /// <summary>
        /// When a server side tractor beam selects, we must
        /// broadcast an update message to all connected clients
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleTractorBeamSelected(Object arg)
        {
            var action = _player.CreateActionMessage("tractor-beam:select");
            _player.Ship.TractorBeam.WriteOffsetData(action);
            _player.Ship.TractorBeam.WriteSelectedData(action);
        }

        /// <summary>
        /// When a server side tractor beam releases, we must
        /// broadcast an update message to all connected clients
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleTractorBeamRelease(Object arg)
        {
            var action = _player.CreateActionMessage("tractor-beam:release");
            _player.Ship.TractorBeam.WriteOffsetData(action);
        }

        /// <summary>
        /// When the server side tractor beam's offset is changed, we
        /// must broadcast an update to all connected clients
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleTractorBeamOffsetChanged(Object arg)
        {
            var action = _player.CreateActionMessage("tractor-beam:set:offset");
            _player.Ship.TractorBeam.WriteOffsetData(action);
        }
        #endregion
    }
}
