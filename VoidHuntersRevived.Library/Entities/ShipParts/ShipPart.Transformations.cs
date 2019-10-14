using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    /// <summary>
    /// Contains the transformation/matrix specific code for
    /// ShipParts.
    /// </summary>
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
        #endregion

        #region Lifecycle Methods
        private void Transformations_PreInitialize()
        {
            this.Events.TryAdd<ChainUpdate>("chain:updated", this.Transformations_HandleChainUpdated);
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

                this.LocalRotation = this.Parent.LocalRotation + female.LocalRotation - male.LocalRotation;

                this.LocalTransformation = Matrix.Invert(male.LocalTransformationMatrix)
                    * female.LocalTransformationMatrix
                    * this.Parent.LocalTransformation;
            }
        }
        #endregion

        #region Event Handlers
        private void Transformations_HandleChainUpdated(object sender, ChainUpdate arg)
        {
            if(arg.HasFlag(ChainUpdate.Down))
                this.UpdateLocalTranslation();
        }
        #endregion
    }
}
