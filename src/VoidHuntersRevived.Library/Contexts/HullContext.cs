using Guppy.Lists;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Attributes;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Contexts
{
    [ShipPartContext("Hull", "Represents a basic static non moving part.")]
    public class HullContext : ShipPartContext
    {
        #region ShipPartContext Implementation
        /// <inheritdoc />
        public override string ShipPartServiceConfiguration => VHR.Entities.Hull;
        #endregion

        #region Constructors
        public HullContext(string name) : base(name)
        {
        }
        #endregion
    }
}
