using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Enums;

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
        /// current chain.
        /// </summary>
        public Single LocalRotation { get; private set; } = 0;
        #endregion

        #region Events
        public event OnEventDelegate<ShipPart> OnTransformationsCleaned;
        #endregion

        #region Lifecycle Methods
        private void Transformations_Create(GuppyServiceProvider provider)
        {
            this.OnTreeClean += ShipPart.Transformations_HandleTreeClean;
        }

        private void Transformations_Dispose()
        {
            this.OnTreeClean -= ShipPart.Transformations_HandleTreeClean;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Update the internal translation values.
        /// </summary>
        protected virtual void CleanLocalTranslation()
        {
            if (this.IsRoot)
            { // If the current part is the root, there is no need to track a translation matrix
                this.LocalTransformation = ShipPart.EmptyTranslation;
                this.LocalRotation = 0;
            }
            else
            { // Calculate the ShipPart's translations based on its parent connection node translations
                this.LocalRotation = MathHelper.WrapAngle(
                    this.ChildConnectionNode.LocalRotation +
                    this.ChildConnectionNode.Connection.Target.LocalRotation +
                    this.ChildConnectionNode.Connection.Target.Owner.LocalRotation);
            
                this.LocalTransformation = this.ChildConnectionNode.LocalChildTransformationMatrix
                    * this.ChildConnectionNode.Connection.Target.LocalTransformationMatrix
                    * this.ChildConnectionNode.Connection.Target.Owner.LocalTransformation;
            }
            
            this.OnTransformationsCleaned?.Invoke(this);
        }
        #endregion

        #region Event Handlers
        private static void Transformations_HandleTreeClean(ShipPart sender, ShipPart source, TreeComponent components)
        {
            if((components & TreeComponent.Node) != 0)
                sender.CleanLocalTranslation();
        }
        #endregion
    }
}
