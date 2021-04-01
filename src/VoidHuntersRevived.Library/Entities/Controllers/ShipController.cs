using Guppy.DependencyInjection;
using Guppy.Extensions.System.Collections;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Extensions.DependencyInjection;
using Lidgren.Network;

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

        #region Public Properties
        /// <inheritdoc />
        public new Int32 DirtyUpdateSequenceChannel => VHR.Network.MessageData.Ship.DirtyUpdate.SequenceChannel;

        /// <inheritdoc />
        public new NetDeliveryMethod DirtyUpdateNetDeliveryMethod => VHR.Network.MessageData.Ship.DirtyUpdate.NetDeliveryMethod;

        public Ship Ship { get; set; }
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

        protected override void Release()
        {
            base.Release();

            _chunks = null;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.chains.TryUpdateAll(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.chains.TryDrawAll(gameTime);
        }
        #endregion

        #region Controller Methods
        internal new void TryAdd(Chain chain)
            => base.TryAdd(chain);

        protected override void Add(Chain chain)
        {
            // Auto remove any other parts..
            while (this.chains.Any())
                this.TryRemove(this.chains.First());

            base.Add(chain);

            // Update the new parts collisions
            foreach(ShipPart shipPart in chain.Root.Items())
            {
                if(shipPart.CollisionCategories == VHR.Categories.PassiveCollisionCategories)
                    shipPart.CollisionCategories = this.Ship.ShipCollisionCategories;

                if (shipPart.CollidesWith == VHR.Categories.PassiveCollidesWith)
                    shipPart.CollidesWith = this.Ship.ShipCollidesWith;
            }
        }
        #endregion
    }
}
