using Guppy.DependencyInjection;
using Guppy.Extensions.Collections;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Library.Entities.Controllers
{
    /// <summary>
    /// Simple controller designed to be used by the Ship class to store
    /// & update the current bridge.
    /// </summary>
    internal sealed class ShipController : Controller
    {
        #region Private Fields
        private ChunkManager _chunks;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _chunks);

            // The controller will be managed directly by its owning Ship.
            this.Visible = false;
            this.Enabled = false;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.parts.ForEach(p => p.TryUpdate(gameTime));
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.parts.ForEach(p => p.TryDraw(gameTime));
        }
        #endregion

        #region Controller Methods
        internal void TryAdd(ShipPart bridge)
        {
            if (this.CanAdd(bridge))
            {
                // Auto remove any other parts..
                while (this.parts.Any())
                    this.Remove(this.parts.First());

                // Add the new bridge in...
                this.Add(bridge);
            }
        }

        protected override void Add(ShipPart shipPart)
        {
            base.Add(shipPart);

            // Update the new parts collisions
            shipPart.CollisionCategories = Categories.ActiveCollisionCategories;
            shipPart.CollidesWith = Categories.ActiveCollidesWith;
            shipPart.IgnoreCCDWith = Categories.ActiveIgnoreCCDWith;
        }

        protected override void Remove(ShipPart shipPart)
        {
            base.Remove(shipPart);

            // Attempt to auto add the ship part back into the chunks...
            _chunks.TryAdd(shipPart);
        }
        #endregion

        #region Helper Methods
        internal void SetAuthorization(GameAuthorization authorization)
            => this.Authorization = authorization;
        #endregion
    }
}
