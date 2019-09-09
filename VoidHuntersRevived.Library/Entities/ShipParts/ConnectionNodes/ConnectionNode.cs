using GalacticFighters.Library.Configurations;
using Guppy;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Entities.ShipParts.ConnectionNodes
{
    public abstract class ConnectionNode : Creatable
    {
        #region Public Attributes
        /// <summary>
        /// The connection node's id. Ids are not unique, but rather indicate their order
        /// within their parent ship part.
        /// </summary>
        public Int32 Id { get; private set; }

        /// <summary>
        /// The ShipPart containing the current connection node
        /// </summary>
        public ShipPart Parent { get; private set; }

        /// <summary>
        /// The rotation relative to the Parent
        /// </summary>
        public Single LocalRotation { get; private set; }

        /// <summary>
        ///  The position relative to the Parent
        /// </summary>
        public Vector2 LocalPosition { get; private set; }

        /// <summary>
        /// Matrix representation of the LocalRotation
        /// </summary>
        public Matrix LocalRotationMatrix { get; private set; }

        /// <summary>
        /// Matrix representation of the LocalPosition
        /// </summary>
        public Matrix LocalPositionMatrix { get; private set; }

        /// <summary>
        /// Matrix representation of the LocalRotation and LocalPosition
        /// </summary>
        public Matrix LocalTransformationMatrix { get; private set; }

        /// <summary>
        /// The current connection node's target, if any.
        /// </summary>
        public ConnectionNode Target { get; private set; }

        /// <summary>
        /// Whether or not the current node is connected to another node.
        /// </summary>
        public Boolean Connected { get { return this.Target != null; } }
        #endregion

        #region Set Methods
        internal void Configure(Int32 id, ShipPart parent, ConnectionNodeConfiguration configuration)
        {
            // Update the id
            this.Id = id;

            // Update the parent
            this.Parent = parent;

            // Set the position & rotation values
            this.LocalPosition = configuration.Position;
            this.LocalRotation = configuration.Rotation;

            // Update the local matrices based on the LocalPosition and LocalRotation values
            this.LocalRotationMatrix = Matrix.CreateRotationZ(this.LocalRotation);
            this.LocalPositionMatrix = Matrix.CreateTranslation(this.LocalPosition.X, this.LocalPosition.Y, 0);

            // A onestep transformation, first move to the rotation position then translate by position offset.
            this.LocalTransformationMatrix = this.LocalRotationMatrix * this.LocalPositionMatrix;
        }
        #endregion
    }
}
