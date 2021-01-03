using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Contexts
{
    public class GunContext : WeaponContext
    {
        /// <inheritdoc />
        public override string ShipPartServiceConfiguration => "entity:ship-part:weapon:gun";

        public GunContext(string name) : base(name)
        {
        }
    }
}
