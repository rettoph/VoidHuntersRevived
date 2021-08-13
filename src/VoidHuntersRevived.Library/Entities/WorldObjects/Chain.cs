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

namespace VoidHuntersRevived.Library.Entities.WorldObjects
{
    /// <summary>
    /// A chain holds a ship part & any connected children parts.
    /// It acts as the link between a virtual ShipPart & the physical
    /// 2d world.
    /// </summary>
    public class Chain : WorldObject
    {
        #region Private Fields
        private ShipPart _root;

        private Vector2 _position;
        private Single _rotation;
        #endregion

        #region Public Properties
        public override Vector2 Position => _position;

        public override Single Rotation => _rotation;

        public ShipPart Root
        {
            get => _root;
            set
            {
                if (this.Status == ServiceStatus.Ready)
                    throw new InvalidOperationException("Unable to update Chain.Root after initialization.");

                _root = value;
                this.OnRootSet?.Invoke(this, _root);
            }
        }
        #endregion

        #region Events
        internal event OnEventDelegate<Chain, ShipPart> OnRootSet;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            this.LayerGroup = Constants.LayersContexts.Chains.Group.GetValue();
            this.ValidateWorldInfoChangeDetected += this.HandleValidateWorldInfoChangeDetected;
        }

        protected override void PostInitialize(GuppyServiceProvider provider)
        {
            base.PostInitialize(provider);

            if(this.Root == default)
                throw new InvalidOperationException("Invalid Chain.Root value.");

            // Update the roots internal Chain reference.
            this.Root.Chain = this;
            this.Root.OnChainChanged += this.HandleRootChainChanged;
        }

        protected override void Release()
        {
            base.Release();

            this.Root.OnChainChanged -= this.HandleRootChainChanged;
            this.ValidateWorldInfoChangeDetected -= this.HandleValidateWorldInfoChangeDetected;

            // Only remove the root's reference to the chain
            // If it is still the root piece.
            if (this.Root.Chain == this)
                this.Root.Chain = default;

            _root = default;
        }
        #endregion

        #region Frame Methods
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // Createa a new matrix representation of the chain's current world position.
            Matrix worldTransformation = Matrix.CreateRotationZ(this.Rotation) 
                * Matrix.CreateTranslation(this.Position.X, this.Position.Y, 0);

            // Auto invoke the Root's draw method.
            // This should case a cascade draw call of the
            // entire chain's parts.
            this.Root.TryDrawAt(gameTime, ref worldTransformation);
        }
        #endregion

        #region Helper Methods
        public override void TrySetTransformation(Vector2 position, float rotation, NetworkAuthorization authorization = NetworkAuthorization.Master)
        {
            _position = position;
            _rotation = rotation;
        }
        #endregion

        #region Network Methods
        public void WriteAll(NetOutgoingMessage om, ShipPartService shipPartService)
        {
            shipPartService.WriteTree(this.Root, om);
        }

        public void ReadAll(NetIncomingMessage im, ShipPartService shipPartService)
        {
            this.Root = shipPartService.ReadTree(im);
        }
        #endregion

        #region Event Handlers
        private void HandleRootChainChanged(ShipPart sender, Chain old, Chain value)
        {
            this.TryRelease();
        }

        private bool HandleValidateWorldInfoChangeDetected(IWorldObject sender, GameTime args)
            => true;
        #endregion
    }
}
