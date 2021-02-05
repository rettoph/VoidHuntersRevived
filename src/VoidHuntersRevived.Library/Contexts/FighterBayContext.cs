using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Attributes;

namespace VoidHuntersRevived.Library.Contexts
{
    [ShipPartContextAttribute("Rigid", "Represents a basic static non moving part.")]
    public class FighterBayContext : RigidShipPartContext
    {
        #region ShipPartContext Implementation
        /// <inheritdoc />
        public override string ShipPartServiceConfiguration => VHR.Entities.FighterBay;
        #endregion

        public FighterBayContext(String name) : base(name)
        {
            this.DefaultColor = Color.Pink;
        }
    }
}
