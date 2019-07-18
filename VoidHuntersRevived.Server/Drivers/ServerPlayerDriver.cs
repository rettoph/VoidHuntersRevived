using Guppy.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.CustomEventArgs;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;

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
            _player.Ship.OnDirectionChanged += this.HandleShipDirectionChanged;
        }

        private void RemoveShipEvents(Ship ship)
        {
            ship.OnDirectionChanged -= this.HandleShipDirectionChanged;
        }

        /// <summary>
        /// When a server side ship's direction is changed, we
        /// must broadcast an update message to all connected
        /// clients.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleShipDirectionChanged(object sender, DirectionChangedEventArgs e)
        {
            var action = _player.CreateActionMessage("set:direction");
            _player.Ship.WriteDirectionData(action, e.Direction);
        }
        #endregion
    }
}
