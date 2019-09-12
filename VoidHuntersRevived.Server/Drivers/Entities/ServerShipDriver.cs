using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Entities.ShipParts;
using Guppy;
using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Server.Drivers.Entities
{
    [IsDriver(typeof(Ship))]
    public class ServerShipDriver : Driver<Ship>
    {
        #region Constructor
        public ServerShipDriver(Ship driven) : base(driven)
        {
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.driven.Events.TryAdd<ShipPart>("bridge:changed", this.HandleBridgeChanged);
        }
        #endregion

        #region Event Handlers
        private void HandleBridgeChanged(object sender, ShipPart bridge)
        {
            this.driven.WriteBridge(this.driven.Actions.Create("bridge:changed"));
        }
        #endregion
    }
}
