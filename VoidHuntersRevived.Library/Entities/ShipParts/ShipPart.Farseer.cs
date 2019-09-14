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
