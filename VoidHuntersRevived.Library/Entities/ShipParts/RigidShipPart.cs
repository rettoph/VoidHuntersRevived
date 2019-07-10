using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Collision.Shapes;
using Guppy.Configurations;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public class RigidShipPart : ShipPart
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
            if (this.Fixture?.Body.BodyId != this.Root.BodyId || _currentChainTranslation != this.LocalTransformation)
            { // Only proceed if the ship-parts chain has been changed...
                if (this.Fixture != null) // Clear the old fixture...
                    this.Fixture.Body.DestroyFixture(this.Fixture);

                // Create the new fixture based on the updated chain translations
                var shape = this.config.Shape.Clone() as PolygonShape;
                _currentChainTranslation = this.LocalTransformation;
                shape.Vertices.Transform(ref _currentChainTranslation);
                this.Fixture = this.Root.CreateFixture(shape, this);
            }
        }
    }
}
