using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Extensions;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    /// <summary>
    /// Partial ShipPart class, handles Transformation methods
    /// for ShipParts.
    /// </summary>
    public partial class ShipPart
    {
        #region Private Fields
        private static Matrix EmptyTranslation = Matrix.CreateTranslation(0, 0, 0);

        private Quaternion _rotationOffset;
        private Vector3 _scaleOffset;
        private Vector3 _translateOffset;
        #endregion

        #region Public Attributes
        /// <summary>
        /// The current ShipPart's translation matrix relative to its current root
        /// </summary>
        public Matrix OffsetTranslationMatrix { get; private set; }

        /// <summary>
        /// The current ShipPart's rotation offset relative to its current root
        /// </summary>
        public Single OffsetRotation { get; private set; }

        /// <summary>
        /// The current rotation matrix based on the current ShipPart's body.
        /// Note, this is only useful on the Root of a chain.
        /// </summary>
        public Matrix RotationMatrix { get; private set; }
        #endregion

        internal void UpdateTransformationData()
        {
            if(this.IsRoot)
            {
                this.OffsetTranslationMatrix = ShipPart.EmptyTranslation;
                this.OffsetRotation = 0;
                this.RotationMatrix = Matrix.CreateRotationZ(this.Body.Rotation);
            }
            else if(this.HasParent)
            {
                // Calculate the translation matrix relative to the root position...
                this.OffsetTranslationMatrix =
                    (this.MaleConnectionNode.LocalTranslationMatrix *
                    this.MaleConnectionNode.Connection.FemaleConnectionNode.LocalTranslationMatrix)
                    * this.Parent.OffsetTranslationMatrix;

                // Decustruct the matric that was just calculated...
                this.OffsetTranslationMatrix.Decompose(out _scaleOffset, out _rotationOffset, out _translateOffset);

                // Extract the Z rotation and update the offset rotation value
                this.OffsetRotation = _rotationOffset.ToAxisAngle().Z;

                // Note, this.RotationMatrix is never used unless ShipPart.IsRoot is true, so no need to update it
            }
            else
            { // This should never happen?
                throw new Exception("Whoa there. This should never ever EVER happen...");
            }
        }
    }
}
