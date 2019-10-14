using VoidHuntersRevived.Library.Configurations;
using Guppy;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities.ShipParts.ConnectionNodes
{
    public abstract class ConnectionNode : Creatable
    {
        #region Public Attributes
        /// <summary>
        /// The connection node's id. Ids are not unique, but rather indicate their order
        /// within their parent ship part.
        /// </summary>
        public new Int32 Id { get; private set; }

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
        public Boolean Attached { get { return this.Target != null; } }

        /// <summary>
        /// The Node's current posotion relative to the world.
        /// </summary>
        public Vector2 WorldPosition { get { return this.Parent.Root.Position + Vector2.Transform(this.LocalPosition, this.Parent.LocalTransformation * Matrix.CreateRotationZ(this.Parent.Root.Rotation)); } }
        
        /// <summary>
        /// The node's current rotation relative to the world.
        /// </summary>
        public Single WorldRotation { get { return this.Parent.Root.Rotation + this.Parent.LocalRotation + this.LocalRotation; } }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            // Register some basic events
            this.Events.Register<ConnectionNode>("attached");
            this.Events.Register<ConnectionNode>("detached");
        }

        public override void Dispose()
        {
            this.Detach();
        }
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

        #region Connection Methods
        public void Attach(ConnectionNode target)
        {
            // First, ensure that there is no pre-existing connection.
            if (this.Attached)
                this.Detach();

            // Create the local attachment
            this.Target = target;

            // Ensure that the target node is attached correctly too...
            if (target.Target != this)
                target.Attach(this);

            // Invoke the event...
            this.Events?.TryInvoke<ConnectionNode>(this, "attached", this.Target);
        }

        public void Detach()
        {
            if (this.Attached)
            { // Only proceed if the current node is connected to something
                var oldTarget = this.Target;
                this.Target = null;
                oldTarget.Detach();

                // Invoke the event...
                this.Events?.TryInvoke<ConnectionNode>(this, "detached", oldTarget);
            }
        }
        #endregion
    }
}
