using GalacticFighters.Library.Entities;
using Guppy;
using Guppy.Attributes;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Client.Library.Drivers.Entities
{
    [IsDriver(typeof(Ship))]
    public class ClientShipDriver : Driver<Ship>
    {
        #region Constructor
        public ClientShipDriver(Ship driven) : base(driven)
        {
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.driven.Actions.TryAdd("bridge:changed", this.HandleBridgeChanged);
        }
        #endregion

        #region Action Handlers
        private void HandleBridgeChanged(object sender, NetIncomingMessage im)
        {
            this.driven.ReadBridge(im);
        }
        #endregion
    }
}
