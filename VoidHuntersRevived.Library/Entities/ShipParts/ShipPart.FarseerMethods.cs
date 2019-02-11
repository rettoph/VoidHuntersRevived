using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    /// <summary>
    /// Partial ShipPart class, handles FarseerSpecific methods
    /// for ShipParts.
    /// </summary>
    public partial class ShipPart
    {
        private static Matrix DefaultTransformationMatrix = Matrix.CreateTranslation(0, 0, 0);

        /// <summary>
        /// Update the current shipparts b
        /// </summary>
        internal Fixture CreateFixture(Body body, Matrix? transformationMatrix = null, Single density = 10f)
        {
            // If the input matrix is null, use the predefined default one instead...
            var matrix = transformationMatrix == null ? ShipPart.DefaultTransformationMatrix : transformationMatrix.Value;

            // Create a new vertices contained based on the current ship part vertices length
            var vertices = new Vertices(this.Data.Vertices.Length);

            // Transform all the current shipparts vertices based on the transformation matrix
            foreach (Vector2 vertice in this.Data.Vertices)
                vertices.Add(Vector2.Transform(vertice, matrix));

            // Create a fixture in the input body and return it
            return FixtureFactory.AttachPolygon(vertices, density, body, this);
        }

        /// <summary>
        /// Create the ship parts body.
        /// </summary>
        internal Body CreateBody()
        {
            // Create a new body
            var body = BodyFactory.CreateBody(world: scene.World, userData: this);

            // Set some body defaults
            body.BodyType = BodyType.Dynamic;
            body.AngularDamping = 1f;
            body.LinearDamping = 1f;

            body.CollidesWith = Category.Cat1;
            body.CollisionCategories = Category.Cat10;

            return body;
        }
    }
}
