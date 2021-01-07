using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Contexts
{
    public class ArmorContext : RigidShipPartContext
    {
        #region ShipPartContext Implementation
        /// <inheritdoc />
        public override string ShipPartServiceConfiguration => "entity:ship-part:armor";
        #endregion

        #region Constructors
        public ArmorContext(string name) : base(name)
        {
            this.DensityMultiplier = 3f;
            this.DefaultColor = Color.Gray;
            this.InheritColor = false;
        }
        #endregion
    }
}
