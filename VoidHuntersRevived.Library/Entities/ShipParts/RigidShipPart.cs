using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Entities.ShipParts
{
    /// <summary>
    /// Implementation of the ShipPart class that creates a rigig attachment
    /// via fixture transplanting.
    /// </summary>
    public class RigidShipPart : ShipPart
    {
        protected override void UpdateChainPlacement()
        {
            // Remove all pre existing fixtures...
            this.body.FixtureList.ForEach(fixture => (fixture.Body.UserData as FarseerEntity).DestroyFixture(fixture));

            // Create new fixtures for all vertices contained in the configuration
            this.config.Vertices.ForEach(data =>
            {
                Vertices vertices = new Vertices(data);
                Matrix _currentChainTranslation = this.LocalTransformation;
                vertices.Transform(ref _currentChainTranslation);
                this.Root.CreateFixture(new PolygonShape(vertices, 0.5f), this);
            });
        }
    }
}
