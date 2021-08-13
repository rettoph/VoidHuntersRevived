using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Common;

namespace VoidHuntersRevived.Library.Dtos.Utilities
{
    /// <summary>
    /// Primarily used to contain overarching shape
    /// data within a ShipPart.
    /// </summary>
    public class ShapeDto
    {
        /// <summary>
        /// The raw vertex data. used to draw or 
        /// simulate shape data.
        /// </summary>
        public Vertices Vertices { get; set; }
    }
}
