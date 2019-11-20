using Guppy;
using Guppy.Attributes;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities
{
    /// <summary>
    /// Manage all recieved ship actions from the server
    /// </summary>
    [IsDriver(typeof(Ship))]
    internal sealed class ShipClientDriver : Driver<Ship>
    {
        #region Constructor
        public ShipClientDriver(Ship driven) : base(driven)
        {
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.driven.Actions.TryAdd("direction:changed", this.HandleDirectionChangedAction);
        }
        #endregion

        #region Action Handlers 
        private void HandleDirectionChangedAction(object sender, NetIncomingMessage arg)
        {
            this.driven.ReadDirection(arg);
        }
        #endregion
    }
}
