﻿using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Collision.Shapes;
using Guppy.Configurations;
using Microsoft.Xna.Framework;
using System.Linq;
using FarseerPhysics.Common;

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
            if (this.Fixture?.Body.BodyId != this.Root.BodyId || _currentChainTranslation != this.LocalTransformation)
            { // Only proceed if the ship-parts chain has been changed...
                if (this.Fixture != null) // Clear the old fixture...
                    (this.Fixture.Body.UserData as FarseerEntity).DestroyFixture(this.Fixture);

                // Create the new fixture based on the updated chain translations
                var vertices = new Vertices(this.config.Vertices.Select(v => new Vector2(v.X, v.Y)));
                _currentChainTranslation = this.LocalTransformation;
                vertices.Transform(ref _currentChainTranslation);
                this.Fixture = this.Root.CreateFixture(new PolygonShape(vertices, this.config.Density), this);
            }
        }
    }
}
