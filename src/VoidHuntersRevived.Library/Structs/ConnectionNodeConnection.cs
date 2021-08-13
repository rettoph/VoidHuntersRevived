using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Structs
{
    public struct ConnectionNodeConnection
    {
        /// <summary>
        /// The default estranged connection.
        /// </summary>
        public static readonly ConnectionNodeConnection DefaultEstranged = new ConnectionNodeConnection(ConnectionNodeState.Estranged, default);

        /// <summary>
        /// The current connection node's state.
        /// </summary>
        public readonly ConnectionNodeState State;

        /// <summary>
        /// The current connection node's target if any.
        /// </summary>
        public readonly ConnectionNode Target;

        internal ConnectionNodeConnection(ConnectionNodeState state, ConnectionNode target)
        {
            this.State = state;
            this.Target = target;
        }

        public override bool Equals(object obj)
        {
            return obj is ConnectionNodeConnection connection &&
                   State == connection.State &&
                   Target == connection.Target;
        }

        public override int GetHashCode()
        {
            int hashCode = 1019353966;
            hashCode = hashCode * -1521134295 + State.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ConnectionNode>.Default.GetHashCode(Target);
            return hashCode;
        }

        public static bool operator ==(ConnectionNodeConnection connection1, ConnectionNodeConnection connection2)
        {
            return connection1.State == connection2.State && connection1.Target == connection2.Target;
        }

        public static bool operator !=(ConnectionNodeConnection connection1, ConnectionNodeConnection connection2) => !(connection1 == connection2);
    }
}
