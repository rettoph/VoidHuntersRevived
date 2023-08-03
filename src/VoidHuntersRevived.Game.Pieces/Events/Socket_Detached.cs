﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Game.Pieces.Events
{
    internal class Socket_Detached : IEventData
    {
        public required VhId CouplingVhId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<Socket_Detached, VhId, VhId>.Instance.Calculate(source, this.CouplingVhId);
        }
    }
}
