using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.Extensions.Utilities;
using Guppy.Utilities;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;
using Guppy.Extensions.System;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Guppy.Enums;
using Guppy.Events.Delegates;
using System.Threading.Tasks;
using System.Collections.Generic;
using Lidgren.Network;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Enums;
using Guppy.Network.Enums;
using VoidHuntersRevived.Library.Entities.Aether;
using System.Linq;
using VoidHuntersRevived.Library.Globals.Constants;

namespace VoidHuntersRevived.Library.Entities.WorldObjects
{
    /// <summary>
    /// A chain holds a ship part & any connected children parts.
    /// It acts as the link between a virtual ShipPart & the physical
    /// 2d world.
    /// </summary>
    public class Chain : AetherBodyWorldObject
    {
        #region Private Fields
        private ShipPart _root;
        #endregion

        #region Public Properties
        public ShipPart Root
        {
            get => _root;
            internal set
            {
                if (this.Status != ServiceStatus.PreInitializing && this.Status != ServiceStatus.PostReleasing)
                    throw new InvalidOperationException($"Unable to update {nameof(Chain)}::{nameof(this.Root)} unless {nameof(this.Status)} is {nameof(ServiceStatus.PreInitializing)} or {nameof(ServiceStatus.PostReleasing)}.");

                _root = value;
            }
        }

        public Color? Color { get; set; }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            this.LayerGroup = LayersContexts.Chains.Group.GetValue();
            this.Corporeal = false; // Chains are non-corporeal by default.

            this.log.Info($"Initializing new Chain {this.Id}");
        }

        protected override void PostInitialize(GuppyServiceProvider provider)
        {
            base.PostInitialize(provider);

            if(this.Root == default)
                throw new ArgumentOutOfRangeException(nameof(this.Root));

            // Update the roots internal Chain reference.
            this.Root.Chain = this;
            this.Root.OnChainChanged += this.HandleRootChainChanged;
        }

        protected override void Release()
        {
            base.Release();

            this.Root.OnChainChanged -= this.HandleRootChainChanged;
        }

        protected override void PostRelease()
        {
            base.PostRelease();

            // Only release the root piece if it is still bound to the chain
            if (this.Root.Chain == this)
            {
                this.Root.TryRelease();
            }

            this.Root = default;
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // Createa a new matrix representation of the chain's current world position.
            Matrix worldTransformation = this.CalculateWorldTransformation();

            // Auto invoke the Root's draw method.
            // This should case a cascade draw call of the
            // entire chain's parts.
            this.Root.TryDrawAt(gameTime, ref worldTransformation);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Calculate the Chain's current WorldMatric based on
        /// <see cref="WorldObject.Rotation"/> and <see cref="WorldObject.Position"/>
        /// </summary>
        /// <returns></returns>
        public Matrix CalculateWorldTransformation()
        {
            return Matrix.CreateRotationZ(this.Rotation) * Matrix.CreateTranslation(this.Position.X, this.Position.Y, 0);
        }
        #endregion

        #region Network Methods
        public void WriteAll(NetOutgoingMessage om, ShipPartService shipPartService)
        {
            shipPartService.TryWriteShipPart(this.Root, om, ShipPartSerializationFlags.CreateTree);
        }

        public void ReadAll(NetIncomingMessage im, ShipPartService shipPartService)
        {
            this.Root = shipPartService.TryReadShipPart(im, ShipPartSerializationFlags.CreateTree);
        }
        #endregion

        #region Event Handlers
        private void HandleRootChainChanged(ShipPart sender, Chain old, Chain value)
        {
            this.TryRelease();
        }
        #endregion
    }
}
