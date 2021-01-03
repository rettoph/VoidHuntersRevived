using Guppy.Extensions.Microsoft.Xna.Framework;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Library.Extensions.Microsoft.Xna;

namespace VoidHuntersRevived.Library.Contexts
{
    /// <summary>
    /// A single <see cref="ShipPart"/> shape,
    /// which contains a single Shapes side angles, 
    /// & connection nodes.
    /// </summary>
    public class ShipPartShapeContext
    {
        #region Structs
        public struct SideContext
        {
            public Single Rotation;
            public Boolean IncludeFemales;
            public Single Length;
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// An array of all internal sides.
        /// </summary>
        public SideContext[] Sides { get; private set; }

        /// <summary>
        /// The calculated vertex data based on the recieved side data.
        /// </summary>
        public Vertices Vertices { get; private set; }
        /// <summary>
        /// The female connection node data, if any, calculated 
        /// from the defined side data.
        /// </summary>
        public ConnectionNodeContext[] FemaleConnectionNodes { get; private set; }

        /// <summary>
        /// The default density to use when creating a 
        /// new <see cref="Fixture"/>.
        /// </summary>
        public Single Density { get; private set; }

        /// <summary>
        /// The scale transformation applied to the defined sides.
        /// </summary>
        public Single Scale { get; private set; }

        /// <summary>
        /// The translation transformation applied to the defined sides.
        /// </summary>
        public Vector2 Translation { get; private set; }

        /// <summary>
        /// The rotation transformation applied to the defined sides.
        /// </summary>
        public Single Rotation { get; private set; }
        #endregion

        #region Constructors
        internal ShipPartShapeContext(
            Vertices vertices, 
            ConnectionNodeContext[] females, 
            SideContext[] sides,
            Single scale,
            Vector2 translation,
            Single rotation,
            Single density)
        {
            this.Sides = sides;
            this.Vertices = vertices;
            this.FemaleConnectionNodes = females;

            this.Scale = scale;
            this.Translation = translation;
            this.Rotation = rotation;
            this.Density = density;
        }
        #endregion
    }
}
