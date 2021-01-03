using Guppy.Lists;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Contexts
{
    public class RigidShipPartContext : ShipPartContext
    {
        #region ShipPartContext Implementation
        /// <inheritdoc />
        public override string ShipPartServiceConfiguration => "entity:ship-part:rigid-ship-part";
        #endregion

        #region Constructors
        public RigidShipPartContext(string name) : base(name)
        {
        }
        #endregion
    }
}
