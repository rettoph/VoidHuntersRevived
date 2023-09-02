using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Ships.Services
{
    public interface ITractorBeamEmitterService
    {
        ref EntityFilterCollection GetTractorableFilter(EntityId tractorBeamEmitterId);
        bool Query(EntityId tractorBeamEmitterId, FixVector2 target, out Node targetNode);

        void Select(EntityId tractorBeamEmitterId, EntityId nodeId);
        void Deselect(EntityId tractorBeamEmitterId);
    }
}
