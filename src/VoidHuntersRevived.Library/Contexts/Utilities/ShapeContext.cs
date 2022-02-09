using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Contexts.ShipParts;
using System.Text.Json.Serialization;

namespace VoidHuntersRevived.Library.Contexts.Utilities
{
    /// <summary>
    /// Primarily used to contain overarching shape
    /// data within a ShipPart.
    /// </summary>
    public class ShapeContext
    {
        /// <summary>
        /// Determins whether or not a shape should be rendered.
        /// </summary>
        public Boolean IsVisible { get; set; } = true;

        /// <summary>
        /// Determins whether or not a shape should have a solid
        /// Aether fixture.
        /// </summary>
        public Boolean IsCorporeal { get; set; } = true;

        /// <summary>
        /// The raw Shape to be used.
        /// </summary>
        [JsonPropertyOrder(1)]
        public Shape Data { get; set; }
    }
}
