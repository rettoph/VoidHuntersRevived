using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Ships.Services
{
    public interface ITractorBeamEmitterService
    {
        bool QueryClosestTarget(EntityId tractorBeamEmitterId, FixVector2 target, out Component<Node> targetNode, out Component<Tree> targetTree);
    }
}
