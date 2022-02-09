using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Structs
{
    /// <summary>
    /// Defines the current state of a tractor beam.
    /// </summary>
    public struct TractorBeamState
    {
        public static readonly TractorBeamState Default = new TractorBeamState(TractorBeamStateType.None, default, default, HostType.Local);

        /// <summary>
        /// The <see cref="TractorBeamStateType"/> this current <see cref="Action"/>
        /// is defining.
        /// </summary>
        public readonly TractorBeamStateType Type;

        /// <summary>
        /// The target<see cref="ShipPart"/> in question this
        /// <see cref="TractorBeamStateType"/> is to be preformed on.
        /// </summary>
        public readonly ShipPart TargetPart;

        /// <summary>
        /// The <see cref="ConnectionNode"/>, if any, this <see cref="TractorBeamStateType"/> is to be
        /// preformed on. This is generally used to defined which node
        /// the <see cref="TargetPart"/> wishes to attach to when
        /// the <see cref="Type"/> is <see cref="TractorBeamStateType.Deselect"/>.
        /// </summary>
        public readonly ConnectionNode DestinationNode;

        /// <summary>
        /// The host that requested the current state.
        /// </summary>
        public readonly HostType RequestHost;

        private TractorBeamState(TractorBeamStateType type, ShipPart targetPart, ConnectionNode destinationNode, HostType requestHost)
        {
            this.Type = type;
            this.TargetPart = targetPart;
            this.DestinationNode = destinationNode;
            this.RequestHost = requestHost;
        }

        public override bool Equals(object obj)
        {
            return obj is TractorBeamState state &&
                   Type == state.Type &&
                   TargetPart?.Id == state.TargetPart?.Id &&
                   DestinationNode?.Id == state.DestinationNode?.Id;
        }

        public static bool operator ==(TractorBeamState left, TractorBeamState right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TractorBeamState left, TractorBeamState right)
        {
            return !(left == right);
        }

        public static TractorBeamState Select(ShipPart target, HostType requestHost)
        {
            return new TractorBeamState(TractorBeamStateType.Select, target, default, requestHost);
        }

        internal static TractorBeamState Deselect(ShipPart targetPart, ConnectionNode destinationNode, HostType requestHost)
        {
            return new TractorBeamState(TractorBeamStateType.Deselect, targetPart, destinationNode, requestHost);
        }
    }
}
