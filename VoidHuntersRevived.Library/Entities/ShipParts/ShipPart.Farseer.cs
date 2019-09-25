using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using GalacticFighters.Library.Configurations;
using GalacticFighters.Library.Utilities;
using Microsoft.Xna.Framework;

namespace GalacticFighters.Library.Entities.ShipParts
{
    /// <summary>
    /// The farseer components within a GalacticFighter's 
    /// ShipPart. This contains *NONE* of the code for
    /// transformations, or connection nodes; but rather
    /// contains miscellaneous farseer integration code. 
    /// </summary>
    public partial class ShipPart : FarseerEntity
    {
        #region Attributes
        /// <summary>
        /// Get the world body origin position.
        /// </summary>
        public new Vector2 Position { get { return this.IsRoot ? base.Position : this.Root.Position + Vector2.Transform(Vector2.Zero, this.LocalTransformation * Matrix.CreateRotationZ(this.Root.Rotation)); } }

        /// <summary>
        /// Get the angle in radians.
        /// </summary>
        public new Single Rotation { get { return this.IsRoot ? base.Rotation : this.Root.Rotation + this.LocalRotation; } }

        /// <summary>
        /// Get the linear velocity of the center of mass.
        /// </summary>
        public new Vector2 LinearVelocity { get { return this.IsRoot ? base.LinearVelocity : this.Root.LinearVelocity; } }

        /// <summary>
        /// Gets the angular velocity. Radians/second.
        /// </summary>
        public new Single AngularVelocity { get { return this.IsRoot ? base.AngularVelocity : this.Root.AngularVelocity; } }

        /// <summary>
        /// Get the world position of the center of mass.
        /// </summary>
        /// <value>The world position.</value>
        public new Vector2 WorldCenter { get { return this.IsRoot ? base.WorldCenter : this.Position + Vector2.Transform(this.LocalCenter, Matrix.CreateRotationZ(this.Rotation)); } }

        /// <summary>
        /// Get the local position of the center of mass.
        /// </summary>
        /// <value>The local position.</value>
        public new Vector2 LocalCenter { get { return this.IsRoot ? base.LocalCenter : this.config.Centeroid; } }
        #endregion

        #region Lifecycle Methods
        private void Farseer_PreInitialize()
        {
            // Setup the chain placement at least once...
            this.UpdateChainPlacement();
        }
        #endregion

        #region FarseerEntity Implementation
        /// <inheritdoc/>
        protected override Body BuildBody(World world)
        {
            return new Body(world, Vector2.Zero, 0, BodyType.Dynamic, this)
            {
                AngularDamping = 1f,
                LinearDamping = 1f
            };
        }
        #endregion

        #region Farseer Methods
        /// <summary>
        /// Invoked when the ShipPart must attach update itself to the current
        /// chain.
        /// </summary>
        protected abstract void UpdateChainPlacement();
        #endregion
    }
}
