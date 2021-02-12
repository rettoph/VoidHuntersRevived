using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Attributes;

namespace VoidHuntersRevived.Library.Contexts
{
    [ShipPartContextAttribute("Shield Generator", "Capable of generating force-field like shields.")]
    public class ShieldGeneratorContext : ShipPartContext
    {
        #region Public Properties
        public override string ShipPartServiceConfiguration => VHR.Entities.ShieldGenerator;

        public Single Radius { get; set; } = 10;
        public Single Range { get; set; } = MathHelper.ToRadians(90);
        #endregion

        #region Constructors 
        public ShieldGeneratorContext(string name) : base(name)
        {
        }
        #endregion
    }
}
