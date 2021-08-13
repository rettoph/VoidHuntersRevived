using Guppy;
using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Guppy.Extensions.DependencyInjection;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Contexts.Utilities;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Structs;

namespace VoidHuntersRevived.Library.Utilities
{
    public class ConnectionNode : Service
    {
        #region Private Fields
        private ConnectionNodeConnection _connection;
        #endregion

        #region Public Properties
        /// <summary>
        /// The <see cref="ConnectionNode"/>'s index within the 
        /// <see cref="ConnectionNode.Owner"/>'s <see cref="ShipPart.ConnectionNodes"/> array
        /// </summary>
        public Int32 Index { get; private set; }

        /// <summary>
        /// The <see cref="ShipPart"/> containing the current <see cref="ConnectionNode"/>.
        /// </summary>
        public ShipPart Owner { get; private set; }

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
        /// when <see cref="State"/> is <see cref="ConnectionNodeState.Parent"/>.
        /// </summary>
        public Matrix LocalRotationMatrix { get; private set; }

        /// <summary>
        /// Matrix representation of the LocalRotation
        /// when <see cref="State"/> is <see cref="ConnectionNodeState.Child"/>.
        /// </summary>
        public Matrix LocalChildRotationMatrix { get; private set; }

        /// <summary>
        /// Matrix representation of the LocalPosition.
        /// </summary>
        public Matrix LocalTranslationMatrix { get; private set; }

        /// <summary>
        /// Matrix representation of the LocalRotation and LocalPosition
        /// when <see cref="State"/> is <see cref="ConnectionNodeState.Parent"/>.
        /// </summary>
        public Matrix LocalTransformationMatrix { get; private set; }

        /// <summary>
        /// Matrix representation of the LocalRotation and LocalPosition
        /// when <see cref="State"/> is <see cref="ConnectionNodeState.Child"/>.
        /// 
        /// Basically, the rotation is fliped 180 degrees.
        /// </summary>
        public Matrix LocalChildTransformationMatrix { get; private set; }

        /// <summary>
        /// The currently defined connection's state & target.
        /// </summary>
        public ConnectionNodeConnection Connection
        {
            get => _connection;
            set => this.OnConnectionChanged.InvokeIf(_connection != value, this, ref _connection, value);
        }
        #endregion

        #region Events
        public OnChangedEventDelegate<ConnectionNode, ConnectionNodeConnection> OnConnectionChanged;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Connection = ConnectionNodeConnection.DefaultEstranged;
        }

        protected override void Release()
        {
            base.Release();

            // Ensure any connection are removed.
            this.TryDetach();
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Attempt to attach a recieved
        /// orphan connection node as a child.
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public Boolean TryAttach(ConnectionNode child)
        {
            // Ensure the current node is an orphan
            if (this.Connection.State != ConnectionNodeState.Estranged)
                return false;

            // Ensure the prospective child is also an orphan
            if (child.Connection.State != ConnectionNodeState.Estranged)
                return false;

            // Ensure the child's owner has no child nodes defined already...
            if (child.Owner.ChildConnectionNode != default)
                return false;

            // Create the attachment!
            this.Connection = new ConnectionNodeConnection(ConnectionNodeState.Parent, child);
            this.Connection.Target.Connection = new ConnectionNodeConnection(ConnectionNodeState.Child, this);

            return true;
        }

        /// <summary>
        /// Attempt to sever any existing connection. 
        /// </summary>
        public Boolean TryDetach()
        {
            if (this.Connection.State == ConnectionNodeState.Estranged)
                return false;

            this.Connection.Target.Connection = ConnectionNodeConnection.DefaultEstranged;
            this.Connection = ConnectionNodeConnection.DefaultEstranged;

            return true;
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Construct a new connection node & update
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="context"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static ConnectionNode Build(GuppyServiceProvider provider, ConnectionNodeDto context, ShipPart owner, Int32 index)
            => provider.GetService<ConnectionNode>((n, p, c) =>
            {
                // Update the parent
                n.Owner = owner;
                n.Index = index;

                // Set the position & rotation values
                n.LocalPosition = context.Position;
                n.LocalRotation = context.Rotation;

                // Update the local matrices based on the LocalPosition and LocalRotation values
                n.LocalTranslationMatrix = Matrix.CreateTranslation(n.LocalPosition.X, n.LocalPosition.Y, 0);
                n.LocalRotationMatrix = Matrix.CreateRotationZ(n.LocalRotation);
                n.LocalChildRotationMatrix = Matrix.CreateRotationZ(n.LocalRotation + MathHelper.Pi);

                // A onestep transformation, first move to the rotation position then translate by position offset.
                n.LocalTransformationMatrix = n.LocalRotationMatrix * n.LocalTranslationMatrix;
                n.LocalChildTransformationMatrix = Matrix.Invert(n.LocalChildRotationMatrix * n.LocalTranslationMatrix);
            });
        #endregion
    }
}
