using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Collision.Shapes;
using Guppy.Configurations;
using Microsoft.Xna.Framework;
using System.Linq;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public abstract class RigidShipPart : ShipPart
    {
        #region Private Fields
        private Matrix _currentChainTranslation;
        #endregion

        #region Constructors
        public RigidShipPart(EntityConfiguration configuration, IServiceProvider provider) : base(configuration, provider)
        {
        }
        public RigidShipPart(Guid id, EntityConfiguration configuration, IServiceProvider provider) : base(id, configuration, provider)
        {
        }
        #endregion

        /// <summary>
        /// Destroy the old fixture, if any, then create a brand new one
        /// directly on the root ship-parts body, creating a rigid
        /// joint.
        /// </summary>
        protected override void UpdateChainPlacement()
        {
            if (this.Fixtures.Count == 0 || this.Fixtures.First()?.Body.BodyId != this.Root.BodyId || _currentChainTranslation != this.LocalTransformation)
            { // Only proceed if the ship-parts chain has been changed...
                foreach(Fixture fixture in this.Fixtures)
                { // Clear all old fixtures...
                    if (fixture != null) // Clear the old fixture...
                        (fixture.Body.UserData as FarseerEntity).DestroyFixture(fixture);
                }

                // Clear all old fixtures...
                this.Fixtures.Clear();
                
                foreach(List<Vector2> verticeSetup in this.config.Vertices)
                {
                    // Create the new fixture based on the updated chain translations
                    var vertices = new Vertices(verticeSetup);
                    _currentChainTranslation = this.LocalTransformation;
                    vertices.Transform(ref _currentChainTranslation);
                    this.Fixtures.Add(this.Root.CreateFixture(new PolygonShape(vertices, this.config.Density), this));
                }

            }
        }
    }
}
