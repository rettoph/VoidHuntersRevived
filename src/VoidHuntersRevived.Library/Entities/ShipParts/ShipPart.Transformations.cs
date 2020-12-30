using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.IO.Extensions.log4net;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public partial class ShipPart
    {
        #region Static Attributes
        /// <summary>
        /// The default ship-part local translation
        /// also used whenever the current ship-part
        /// is the rootmost piece
        /// </summary>
        private static Matrix EmptyTranslation = Matrix.CreateTranslation(0, 0, 0);
        #endregion

        #region Public Attributes
        /// <summary>
        /// The ship-parts translation relative to its 
        /// current root
        /// </summary>
        public Matrix LocalTransformation { get; private set; } = ShipPart.EmptyTranslation;

        /// <summary>
        /// The ship-parts rotation relative to its 
        /// current root
        /// </summary>
        public Single LocalRotation { get; private set; } = 0;

        public virtual Matrix WorldTransformation
        {
            get => this.LocalTransformation * Matrix.CreateRotationZ(this.Root.Rotation) * Matrix.CreateTranslation(this.Root.Position.X, this.Root.Position.Y, 0);
        }
        #endregion

        #region Events
        public event OnEventDelegate<ShipPart> OnTransformationsCleaned;
        #endregion

        #region Lifecycle Methods
        private void Transformations_Create(ServiceProvider provider)
        {
            this.OnChainChanged += ShipPart.Transformations_HandleChainChanged;
        }

        private void Transformations_Dispose()
        {
            this.OnChainChanged -= ShipPart.Transformations_HandleChainChanged;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Update the internal translation values.
        /// </summary>
        protected virtual void UpdateLocalTranslation()
        {
            if (this.IsRoot)
            { // If the current part is the root, there is no need to track a translation matrix
                this.LocalTransformation = ShipPart.EmptyTranslation;
                this.LocalRotation = 0;
            }
            else
            { // Calculate the ShipPart's translations based on its parent connection node translations
                var female = this.MaleConnectionNode.Target;
                var male = this.MaleConnectionNode;

                this.LocalRotation = MathHelper.WrapAngle(this.Parent.LocalRotation + female.LocalRotation - male.LocalRotation);

                this.LocalTransformation = Matrix.Invert(male.LocalTransformationMatrix)
                    * female.LocalTransformationMatrix
                    * this.Parent.LocalTransformation;
            }

            this.OnTransformationsCleaned?.Invoke(this);
        }
        #endregion

        #region Event Handlers
        private static void Transformations_HandleChainChanged(ShipPart sender, Chain old, Chain value)
            => sender.UpdateLocalTranslation();
        #endregion
    }
}
