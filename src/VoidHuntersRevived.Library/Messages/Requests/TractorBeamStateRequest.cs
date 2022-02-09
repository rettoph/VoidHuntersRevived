using Guppy.Network.Enums;
using Guppy.Threading.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Messages.Requests
{
    /// <summary>
    /// Defines <see cref="TractorBeam"/> specific action data
    /// that contains required data to preform a desired <see cref="ActionType"/>.
    /// </summary>
    public sealed class TractorBeamStateRequest : ConsolidableMessage
    {
        /// <summary>
        /// The <see cref="TractorBeamStateType"/> this current <see cref="Action"/>
        /// is defining.
        /// </summary>
        public TractorBeamStateType Type { get; init; }

        /// <summary>
        /// The target<see cref="ShipPart"/> in question this
        /// <see cref="TractorBeamStateType"/> is to be preformed on.
        /// </summary>
        public ShipPart TargetPart { get; init; }

        /// <summary>
        /// The <see cref="ConnectionNode"/>, if any, this <see cref="TractorBeamStateType"/> is to be
        /// preformed on. This is generally used to defined which node
        /// the <see cref="TargetPart"/> wishes to attach to when
        /// the <see cref="Type"/> is <see cref="TractorBeamStateType.Deselect"/>.
        /// </summary>
        public ConnectionNode DestinationNode { get; init; }
    }
}
