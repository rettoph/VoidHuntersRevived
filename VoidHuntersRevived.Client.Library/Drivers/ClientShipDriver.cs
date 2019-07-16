using Guppy.Collections;
using Guppy.Implementations;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Client.Library.Drivers
{
    public class ClientShipDriver : Driver
    {
        #region Private Fields
        private Ship _ship;
        #endregion

        #region Constructors
        public ClientShipDriver(Ship parent, IServiceProvider provider) : base(parent, provider)
        {
            _ship = parent;
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            _ship.ActionHandlers["set:bridge"] = this.HandleSetBridgeAction;
        }
        #endregion

        #region Action Handlers
        /// <summary>
        /// When the ship recieves a set bridge data we must
        /// extract the data from the incoming message then
        /// set the bridge value
        /// </summary>
        /// <param name="obj"></param>
        private void HandleSetBridgeAction(NetIncomingMessage obj)
        {
            _ship.ReadBridgeData(obj);
        }
        #endregion

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
