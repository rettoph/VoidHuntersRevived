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
using FarseerPhysics.Dynamics;
using VoidHuntersRevived.Library.Extensions.Farseer;
using Guppy.Events.Delegates;
using Lidgren.Network;

namespace VoidHuntersRevived.Library.Utilities
{
    public sealed class ConnectionNode : Service
    {
        #region Public Properties
        /// <summary>
        /// The nodes index within the parent array
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
        public Vector2 WorldPosition => this.GetWorldPosition(this.Parent.Root.live);

        /// <summary>
        /// The node's current rotation relative to the world.
        /// </summary>
        public Single WorldRotation => this.GetWordRotation(this.Parent.Root.live);
        #endregion

        #region Events
        public OnEventDelegate<ConnectionNode, ConnectionNode> OnAttached;
        public OnEventDelegate<ConnectionNode, ConnectionNode> OnDetached;
        #endregion

        #region Lifecycle Methods
        protected override void Release()
        {
            base.Release();

            if(this.Attached)
            { // Auto detach if needed...
                this.TryDetach(false);
            }
        }
        #endregion

        #region Helper Methods
        public void TryAttach(ConnectionNode target, Boolean align = true)
        {
            if(target != this && target != this.Target)
            { // Only proceed if the connection is a valid non-existing one
                // First ensure that there is no pre-existing connection...
                if (this.Attached)
                    this.TryDetach(align);

                // Create the local attachment
                this.Target = target;

                if (align)
                    target.TryPreview(this.Parent);

                // Ensure that the connection runs both ways
                if (target.Target != this)
                    target.TryAttach(this, false);

                // Trigger the attachment event...
                this.OnAttached?.Invoke(this, this.Target);
            }
        }

        public void TryDetach(Boolean align = true)
        {
            if (this.Attached)
            { // Only proceed if there is an existing attachment
                var old = this.Target;

                this.Target = null;
                old.TryDetach(!align);

                this.OnDetached?.Invoke(this, old);

                if(align)
                    old.TryPreview(this.Parent);
            }
        }

        /// <summary>
        /// Reposition the input ShipPart as if it were to
        /// attach to the current node.
        /// </summary>
        public void TryPreview(ShipPart shipPart)
        {
            shipPart.Do(b =>
            {
                var root = this.Parent.Root.GetChild(shipPart.GetParent(b));
                var rotation = MathHelper.WrapAngle(this.GetWordRotation(root) - shipPart.MaleConnectionNode.LocalRotation);

                b.SetTransformIgnoreContacts(
                    position: this.GetWorldPosition(root) - Vector2.Transform(shipPart.MaleConnectionNode.LocalPosition, Matrix.CreateRotationZ(rotation)),
                    angle: rotation);
            });
        }

        public Vector2 GetWorldPosition(Body root)
            => root.Position + Vector2.Transform(this.LocalPosition, this.Parent.LocalTransformation * Matrix.CreateRotationZ(root.Rotation));

        public Single GetWordRotation(Body root)
            => root.Rotation + this.Parent.LocalRotation + this.LocalRotation;
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
                n.Id = id;

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
