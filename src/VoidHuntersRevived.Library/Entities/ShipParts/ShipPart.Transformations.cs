using Guppy.EntityComponent.DependencyInjection;
using Guppy.Events.Delegates;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;

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
        private void Transformations_Initialize(ServiceProvider provider)
        {
            this.OnTreeClean += ShipPart.Transformations_HandleTreeClean;
        }

        private void Transformations_Uninitialize()
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
                this.LocalRotation = ShipPart.CalculateLocalRotation(
                    child: this.ChildConnectionNode,
                    parent: this.ChildConnectionNode.Connection.Target);

                this.LocalTransformation = ShipPart.CalculateLocalTransformation(
                    child: this.ChildConnectionNode, 
                    parent: this.ChildConnectionNode.Connection.Target);
            }
            
            this.OnTransformationsCleaned?.Invoke(this);
        }

        /// <summary>
        /// Generate a <see cref="Matrix"/> to calculate the <see cref="ShipPart"/> world
        /// data.
        /// </summary>
        /// <param name="chainWorldTransformation">A custom owning world matrix</param>
        /// <returns></returns>
        public Matrix CalculateWorldTransformation(ref Matrix chainWorldTransformation)
        {
            return this.LocalTransformation * chainWorldTransformation;
        }

        /// <summary>
        /// Generate a <see cref="Matrix"/> to calculate the <see cref="ShipPart"/> world
        /// data.
        /// </summary>
        /// <param name="chainWorldTransformation">A custom owning world matrix</param>
        /// <returns></returns>
        public Matrix CalculateWorldTransformation(Matrix chainWorldTransformation)
        {
            return this.CalculateWorldTransformation(ref chainWorldTransformation);
        }

        /// <summary>
        /// Generate a <see cref="Matrix"/> to calculate the <see cref="ShipPart"/> world
        /// data. Automatically use the current <see cref="Chain"/> (if any) for the 
        /// chainWorldMatrix
        /// </summary>
        /// <returns></returns>
        public Matrix CalculateWorldTransformation()
        {
            return this.CalculateWorldTransformation(this.Chain.WorldTransformation);
        }

        /// <summary>
        /// Convert a local point into a world point.
        /// </summary>
        /// <param name="localPoint"></param>
        /// <param name="chainWorldTransformation"></param>
        /// <returns></returns>
        public Vector2 CalculateWorldPoint(Vector2 localPoint, ref Matrix chainWorldTransformation)
        {
            return Vector2.Transform(
                position: localPoint,
                matrix: this.CalculateWorldTransformation(chainWorldTransformation));
        }

        /// <summary>
        /// Convert a local point into a world point.
        /// </summary>
        /// <param name="localPoint"></param>
        /// <param name="chainWorldTransformation"></param>
        /// <returns></returns>
        public Vector2 CalculateWorldPoint(Vector2 localPoint, Matrix chainWorldTransformation)
        {
            return this.CalculateWorldPoint(localPoint, ref chainWorldTransformation);
        }

        /// <summary>
        /// Convert a local point into a world point.
        /// </summary>
        /// <param name="localPoint"></param>
        /// <param name="chainWorldTransformation"></param>
        /// <returns></returns>
        public Vector2 CalculateWorldPoint(Vector2 localPoint)
        {
            return this.CalculateWorldPoint(localPoint, this.Chain.WorldTransformation);
        }
        #endregion

        #region Event Handlers
        private static void Transformations_HandleTreeClean(ShipPart sender, ShipPart source, TreeComponent components)
        {
            if((components & TreeComponent.Node) != 0)
                sender.CleanLocalTranslation();
        }
        #endregion

        #region Static Methods
        public static Single CalculateLocalRotation(ConnectionNode child, ConnectionNode parent)
        {
            return MathHelper.WrapAngle(
                    MathHelper.Pi -
                    child.LocalRotation +
                    parent.LocalRotation +
                    parent.Owner.LocalRotation);
        }

        public static Matrix CalculateLocalTransformation(ConnectionNode child, ConnectionNode parent)
        {
            return child.LocalChildTransformationMatrix
                    * parent.LocalTransformationMatrix
                    * parent.Owner.LocalTransformation;
        }
        #endregion
    }
}
