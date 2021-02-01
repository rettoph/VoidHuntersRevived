using Guppy.Lists;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Attributes;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Contexts
{
    [ShipPartContext("Rigid", "Represents a basic static non moving part.")]
    public class RigidShipPartContext : ShipPartContext
    {
        #region ShipPartContext Implementation
        /// <inheritdoc />
        public override string ShipPartServiceConfiguration => VHR.Entities.RigidShipPart;
        #endregion

        #region Constructors
        public RigidShipPartContext(string name) : base(name)
        {
        }
        #endregion
    }
}
