using Guppy;
using Guppy.Attributes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    /// <summary>
    /// Manage the internal ship controller & tractor beam
    /// </summary>
    [IsDriver(typeof(Ship), 50)]
    internal sealed class ShipComponentsDriver : Driver<Ship>
    {
        public ShipComponentsDriver(Ship driven) : base(driven)
        {
        }

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.driven.TractorBeam.TryDraw(gameTime);
            this.driven.controller.TryDraw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            // Update internal components
            this.driven.TractorBeam.TryUpdate(gameTime);
            this.driven.controller.TryUpdate(gameTime);
        }
            #endregion
        }
}
