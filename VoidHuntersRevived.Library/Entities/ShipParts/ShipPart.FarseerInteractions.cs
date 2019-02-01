using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public partial class ShipPart
    {
        #region Create Shape Methods
        protected Shape CreateShape()
        {
            return new PolygonShape(new Vertices(this.ShipPartData.Vertices), 10f);
        }
        protected Shape CreateShape(Matrix transformation)
        {
            var vertices = new Vertices(this.ShipPartData.Vertices.Length);

            foreach (Vector2 vertice in this.ShipPartData.Vertices)
                vertices.Add(Vector2.Transform(vertice, transformation));

            return new PolygonShape(vertices, 10f);
        }
        #endregion

        #region CreateFixture Methods
        /// <summary>
        /// Update the current live fixure object
        /// and add it to the root most body
        /// </summary>
        /// <returns></returns>
        protected Fixture UpdateFixture()
        {
            if (this.Fixture != null) // Remove the old fixture if there is one
                this.Fixture.Body.DestroyFixture(this.Fixture);

            this.Fixture = this.Root.Body.CreateFixture(this.CreateShape(this.TransformationOffsetMatrix), this);
            this.Fixture.CollidesWith = this.Root.Fixture.CollidesWith;
            this.Fixture.CollisionCategories = this.Root.Fixture.CollisionCategories;

            return this.Fixture;
        }
        #endregion
    }
}
