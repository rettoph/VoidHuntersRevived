using Guppy;
using Guppy.DependencyInjection;
using Guppy.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Extensions.System;
using Guppy.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Library.Utilities
{
    public sealed class ConnectionNode : Service
    {
        #region Public Properties
        /// <summary>
        /// The nodes index within the parent array
        /// </summary>
        public Int32 Index { get; private set; }
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
        /// Matrix representation of the LocalRotation and LocalPosition
        /// </summary>
        public Matrix LocalTransformationMatrix { get; private set; }

        /// <summary>
        /// Matrix representation of the LocalPosition
        /// </summary>
        public Matrix LocalPositionMatrix { get; private set; }


        /// <summary>
        /// The current connection node's target if any.
        /// </summary>
        public ConnectionNode Target { get; private set; }

        /// <summary>
        /// The current attachment state of the node
        /// </summary>
        public Boolean Attached => this.Target != null;

        /// <summary>
        /// The Node's current posotion relative to the world.
        /// </summary>
        public Vector2 WorldPosition { get => this.Parent.Root.Position + Vector2.Transform(this.LocalPosition, this.Parent.LocalTransformation * Matrix.CreateRotationZ(this.Parent.Root.Rotation)); }

        /// <summary>
        /// The node's current rotation relative to the world.
        /// </summary>
        public Single WorldRotation { get => this.Parent.Root.Rotation + this.Parent.LocalRotation + this.LocalRotation; }
        #endregion

        #region Events
        public GuppyEventHandler<ConnectionNode, ConnectionNode> OnAttached;
        public GuppyEventHandler<ConnectionNode, ConnectionNode> OnDetached;
        #endregion

        #region Lifecycle Methods
        protected override void Release()
        {
            base.Release();

            if(this.Attached)
            { // Auto detach if needed...
                this.TryDetach();
            }
        }
        #endregion

        #region Helper Methods
        public void TryAttach(ConnectionNode target)
        {
            if(target != this && target != this.Target)
            { // Only proceed if the connection is a valid non-existing one
                // First ensure that there is no pre-existing connection...
                if (this.Attached)
                    this.TryDetach();

                // Create the local attachment
                this.Target = target;

                // Ensure that the connection runs both ways
                if (target.Target != this)
                    target.TryAttach(this);

                // Trigger the attachment event...
                this.OnAttached?.Invoke(this, this.Target);
            }
        }

        public void TryDetach()
        {
            if (this.Attached)
            { // Only proceed if there is an existing attachment
                var old = this.Target;

                this.Target = null;
                old.TryDetach();

                this.OnDetached?.Invoke(this, old);
            }
        }

        /// <summary>
        /// Reposition the input ShipPart as if it were to
        /// attach to the current node.
        /// </summary>
        public void TryPreview(ShipPart shipPart)
        {
            var rotation = this.WorldRotation - shipPart.MaleConnectionNode.LocalRotation;

            shipPart.SetTransformIgnoreContacts(
                position: this.WorldPosition - Vector2.Transform(shipPart.MaleConnectionNode.LocalPosition, Matrix.CreateRotationZ(rotation)),
                angle: rotation);
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Construct a new connection node & update
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="configuration"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static ConnectionNode Build(ServiceProvider provider, ConnectionNodeConfiguration configuration, ShipPart parent, Int32 id)
            => provider.GetService<ConnectionNode>((n, p, c) =>
            {
                // Update the parent
                n.Parent = parent;
                n.Id = id.ToGuid();
                n.Index = id;

                // Set the position & rotation values
                n.LocalPosition = configuration.Position;
                n.LocalRotation = configuration.Rotation;

                // Update the local matrices based on the LocalPosition and LocalRotation values
                n.LocalRotationMatrix = Matrix.CreateRotationZ(n.LocalRotation);
                n.LocalPositionMatrix = Matrix.CreateTranslation(n.LocalPosition.X, n.LocalPosition.Y, 0);

                // A onestep transformation, first move to the rotation position then translate by position offset.
                n.LocalTransformationMatrix = n.LocalRotationMatrix * n.LocalPositionMatrix;
            });
        #endregion
    }
}
