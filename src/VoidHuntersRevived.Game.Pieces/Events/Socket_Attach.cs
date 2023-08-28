using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Game.Pieces.Events
{
    internal class Socket_Attach : IEventData
    {
        public required SocketVhId SocketVhId { get; init; }
        public required EntityData NodeData { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<Socket_Attach, VhId, SocketVhId, VhId>.Instance.Calculate(source, this.SocketVhId, this.NodeData.Id);
        }
    }
}
