using Guppy.Implementations;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.CustomEventArgs;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Server.Drivers
{
    public class ServerPlayerDriver : Driver
    {
        private Player _player;

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

            _player.OnShipChanged += this.HandleShipChanged;
        }
        #endregion

        #region Event Handlers
        private void HandleShipChanged(object sender, ChangedEventArgs<Ship> e)
        {
            var action = _player.CreateActionMessage("set:ship");
            _player.WriteShipData(action);

            // Remove any old ship events, and add any new ones
            if (e.Old != null)
                this.RemoveShipEvents(e.Old);
            if (e.New != null)
                this.AddShipEvents();
        }

        private void AddShipEvents()
        {
            _player.Ship.OnDirectionChanged += this.HandleDirectionChanged;
            _player.Ship.TractorBeam.OnSelected += this.HandleTractorBeamSelected;
            _player.Ship.TractorBeam.OnReleased += this.HandleTractorBeamRelease;
            _player.Ship.TractorBeam.OnOffsetChanged += this.HandleTractorBeamOffsetChanged;
        }

        private void RemoveShipEvents(Ship ship)
        {
            ship.OnDirectionChanged -= this.HandleDirectionChanged;
            ship.TractorBeam.OnSelected -= this.HandleTractorBeamSelected;
            ship.TractorBeam.OnReleased -= this.HandleTractorBeamRelease;
            ship.TractorBeam.OnOffsetChanged -= this.HandleTractorBeamOffsetChanged;
        }

        /// <summary>
        /// When a server side ship's direction is changed, we
        /// must broadcast an update message to all connected
        /// clients.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleDirectionChanged(object sender, DirectionChangedEventArgs e)
        {
            var action = _player.CreateActionMessage("set:direction");
            _player.Ship.WriteDirectionData(action, e.Direction);
        }

        /// <summary>
        /// When a server side tractor beam selects, we must
        /// broadcast an update message to all connected clients
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleTractorBeamSelected(object sender, ShipPart e)
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
        private void HandleTractorBeamRelease(object sender, ShipPart e)
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
        private void HandleTractorBeamOffsetChanged(object sender, Vector2 e)
        {
            var action = _player.CreateActionMessage("tractor-beam:set:offset");
            _player.Ship.TractorBeam.WriteOffsetData(action);
        }
        #endregion
    }
}
