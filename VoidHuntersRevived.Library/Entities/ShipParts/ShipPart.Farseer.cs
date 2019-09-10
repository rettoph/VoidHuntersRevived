using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using GalacticFighters.Library.Configurations;
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
        #region Events

        #endregion

        #region Lifecycle Methods
        private void Farseer_PreInitialize()
        {
            // Setup the chain placement at least once...
            this.UpdateChainPlacement();
        }

        private void Farseer_Dispose()
        {
            // Remove all fixtures contained within the ship part.
            while (this.body.FixtureList.Any())
                this.body.DestroyFixture(this.body.FixtureList[0]);
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
