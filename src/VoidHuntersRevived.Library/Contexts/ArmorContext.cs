using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Attributes;

namespace VoidHuntersRevived.Library.Contexts
{
    [ShipPartContextAttribute("Armor", "Basic armor")]
    public class ArmorContext : RigidShipPartContext
    {
        #region ShipPartContext Implementation
        /// <inheritdoc />
        public override string ShipPartServiceConfiguration => VHR.Entities.Armor;
        #endregion

        #region Constructors
        public ArmorContext(string name) : base(name)
        {
            this.Density = 3f;
            this.DefaultColor = Color.Gray;
            this.InheritColor = false;
        }
        #endregion
    }
}
