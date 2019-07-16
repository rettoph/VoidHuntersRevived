using Guppy.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.CustomEventArgs;

namespace VoidHuntersRevived.Server.Drivers
{
    public class ServerShipDriver : Driver
    {
        #region Private Fields
        private Ship _ship;
        #endregion

        #region Constructors
        public ServerShipDriver(Ship parent, IServiceProvider provider) : base(parent, provider)
        {
            _ship = parent;
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            _ship.OnBridgeChanged += this.HandleBridgeChanged;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// When the ship's bridge is changed on the server, we
        /// must broadcast a message alerting all connected clients
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleBridgeChanged(object sender, ChangedEventArgs<ShipPart> e)
        {
            var action = _ship.CreateActionMessage("set:bridge", true);
            _ship.WriteBridgeData(action);
        }
        #endregion

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
