using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Entities.ShipParts;
using Guppy;
using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Network.Extensions.Lidgren;

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
            this.driven.Events.TryAdd<Ship.Direction>("direction:changed", this.HandleDirectionChanged);
            this.driven.TractorBeam.Events.TryAdd<ShipPart>("selected", this.HandleTractorBeamSelected);
            this.driven.TractorBeam.Events.TryAdd<ShipPart>("released", this.HandleTractorBeamReleased);
        }
        #endregion

        #region Event Handlers
        private void HandleBridgeChanged(object sender, ShipPart bridge)
        {
            this.driven.WriteBridge(this.driven.Actions.Create("bridge:changed"));
        }

        private void HandleDirectionChanged(object sender, Ship.Direction direction)
        {
            this.driven.WriteDirection(this.driven.Actions.Create("direction:changed"), direction);
        }

        private void HandleTractorBeamSelected(object sender, ShipPart arg)
        {
            var action = this.driven.Actions.Create("tractor-beam:selected");
            action.Write(this.driven.TractorBeam.Offset);
            action.Write(this.driven.TractorBeam.Selected.Id);
        }

        private void HandleTractorBeamReleased(object sender, ShipPart arg)
        {
            var action = this.driven.Actions.Create("tractor-beam:released");
            action.Write(this.driven.TractorBeam.Offset);
        }
        #endregion
    }
}
