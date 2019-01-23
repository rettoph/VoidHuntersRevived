using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Interfaces;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Entities.Ships
{
    public class Ship : FarseerEntity
    {
        public ITractorBeam TractorBeam { get; protected set; }
        public ShipPart Bridge;

        public Ship(EntityInfo info, IGame game) : base(info, game)
        {
            this.Enabled = true;
            this.Visible = false;
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Create a new tractor beam for the ship
            this.TractorBeam = this.Scene.Entities.Create<TractorBeam>("entity:tractor_beam", null, this);
        }
    }
}
