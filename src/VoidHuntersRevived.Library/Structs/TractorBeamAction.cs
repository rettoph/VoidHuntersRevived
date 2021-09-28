using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Structs
{
    /// <summary>
    /// Defines <see cref="TractorBeam"/> specific action data
    /// that contains required data to preform a desired <see cref="ActionType"/>.
    /// </summary>
    public struct TractorBeamAction
    {
        /// <summary>
        /// The <see cref="ActionType"/> this current <see cref="Action"/>
        /// is defining.
        /// </summary>
        public readonly TractorBeamActionType Type;

        /// <summary>
        /// The target<see cref="ShipPart"/> in question this <see cref="Type"/>
        /// <see cref="ActionType"/> is to be preformed on.
        /// </summary>
        public readonly ShipPart TargetPart;

        /// <summary>
        /// The <see cref="ConnectionNode"/>, if any, this <see cref="ActionType"/> is to be
        /// preformed on. This is generally used to defined which node
        /// the <see cref="TargetPart"/> wishes to attach to when
        /// the <see cref="Type"/> is <see cref="ActionType.Attach"/>.
        /// </summary>
        public ConnectionNode TargetNode;

        public TractorBeamAction(TractorBeamActionType type = TractorBeamActionType.None, ShipPart targetShipPart = default, ConnectionNode targetNode = default)
        {
            this.Type = type;
            this.TargetPart = targetShipPart;
            this.TargetNode = targetNode;
        }
    }
}
