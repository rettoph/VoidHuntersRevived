using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Entities.ShipParts
{
    public partial class ShipPart
    {
        /// <summary>
        /// The default ship-part local translation
        /// also used whenever the current ship-part
        /// is the rootmost piece
        /// </summary>
        private static Matrix EmptyTranslation = Matrix.CreateTranslation(0, 0, 0);

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

        #region Methods
        protected virtual void UpdateLocalTranslation()
        {
            if (this.IsRoot)
            { // If the current part is the root, there is no need to track a translation matrix
                this.LocalTransformation = ShipPart.EmptyTranslation;
                this.LocalRotation = 0;
            }
            else
            {
                var female = this.MaleConnectionNode.Target;
                var male = this.MaleConnectionNode;

                this.LocalRotation = this.Parent.LocalRotation + female.LocalRotation - male.LocalRotation;

                this.LocalTransformation = Matrix.Invert(male.LocalTransformationMatrix)
                    * female.LocalTransformationMatrix
                    * this.Parent.LocalTransformation;
            }
        }
        #endregion
    }
}
