using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Connections;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public partial class ShipPart
    {
        #region Public Attributes
        public TractorBeamConnection Connection { get; private set; }
        #endregion

        #region Events
        public event EventHandler<TractorBeamConnection> OnConnected;
        public event EventHandler<ShipPart> OnDisconnected;
        #endregion

        /// <summary>
        /// Attempt to connect to a given TractorBeamConnection
        /// </summary>
        /// <param name="connection"></param>
        internal virtual void Connect(TractorBeamConnection connection)
        {
            if (connection.Status != ConnectionStatus.Connecting)
                throw new Exception("Unable to bind ShipPart to requested connection. Invalid Connection Status.");
            else if (this.Connection != null)
                throw new Exception("Unable to bind ShipPart to requested connection. Node already bound to a connection.");
            else if (connection.ShipPart != this)
                throw new Exception("Unable to bind ShipPart to requested connection. Irrelevant connection.");

            // Save the new connection
            this.Connection = connection;

            // Enable the current ShipPart
            this.SetEnabled(true);
            // Wake up the current body
            this.Body.Awake = true;
            // Disable sleeping for the current body
            this.Body.SleepingAllowed = false;

            // Invoke the OnConnected event
            this.OnConnected?.Invoke(this, connection);
        }

        /// <summary>
        /// Attempt to disconnect from the current NodeConnection
        /// </summary>
        internal virtual void Disconnect()
        {
            if (this.Connection == null)
                throw new Exception("Unable to un-bind ShipPart from current connection. No current connection.");
            else if (this.Connection.Status != ConnectionStatus.Disconnecting)
                throw new Exception("Unable to un-bind ShipPart from current connection. Invalid Connection Status.");

            // Discard the old connection
            this.Connection = null;

            // Re-Enable sleeping for the current body
            this.Body.SleepingAllowed = true;

            // Invoke the OnDisconnected event
            this.OnDisconnected?.Invoke(this, this);
        }
    }
}
