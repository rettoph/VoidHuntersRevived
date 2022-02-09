using Guppy;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Events.Delegates;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Contexts.Utilities;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Structs;
using Guppy.EntityComponent;

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
        public Byte Index { get; private set; }

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

        /// <summary>
        /// The network id of the current ConnectionNode. 
        /// This will allow us to pinpoinnt the exact instance across a network
        /// with minimum data.
        /// </summary>
        public ConnectionNodeNetworkId NetworkId => new ConnectionNodeNetworkId(this.Owner.NetworkId, this.Index);
        #endregion

        #region Events
        public OnChangedEventDelegate<ConnectionNode, ConnectionNodeConnection> OnConnectionChanged;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Connection = ConnectionNodeConnection.DefaultEstranged;
        }

        protected override void Uninitialize()
        {
            base.Uninitialize();

            // Cascade the releasing downward.
            if (this.Connection.State != ConnectionNodeState.Estranged)
                this.Connection.Target.Owner.Dispose();

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
            if (child?.Connection.State != ConnectionNodeState.Estranged)
                return false;

            // Ensure the child's owner has no child nodes defined already...
            if (child.Owner.ChildConnectionNode != default)
                return false;

            // Ensure the child and pareent are not on the same part
            if (child.Owner.Id == this.Owner.Id)
                return false;

            // Create the attachment!
            ConnectionNode.LinkNodes(child, this);

            return true;
        }

        /// <summary>
        /// Attempt to sever any existing connection. 
        /// </summary>
        public Boolean TryDetach()
        {
            if (this.Connection.State == ConnectionNodeState.Estranged)
                return false;

            if(this.Connection.State == ConnectionNodeState.Child)
            {
                ConnectionNode.SeverNodes(this, this.Connection.Target);
            }
            else if(this.Connection.Target.Connection.State == ConnectionNodeState.Child)
            {
                ConnectionNode.SeverNodes(this.Connection.Target, this);
            }
            else
            {
                throw new InvalidOperationException("No Child connection detected.");
            }

            return true;
        }

        private static void SeverNodes(ConnectionNode child, ConnectionNode parent)
        {
            child.Connection = ConnectionNodeConnection.DefaultEstranged;
            parent.Connection = ConnectionNodeConnection.DefaultEstranged;
        }

        private static void LinkNodes(ConnectionNode child, ConnectionNode parent)
        {
            parent.Connection = new ConnectionNodeConnection(ConnectionNodeState.Parent, child);
            child.Connection = new ConnectionNodeConnection(ConnectionNodeState.Child, parent);
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
        public static ConnectionNode Build(ServiceProvider provider, ConnectionNodeContext context, ShipPart owner, Byte index)
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
