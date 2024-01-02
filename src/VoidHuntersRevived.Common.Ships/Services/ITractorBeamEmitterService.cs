using Svelto.ECS;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Ships.Services
{
    public interface ITractorBeamEmitterService
    {
        ref EntityFilterCollection GetTractorableFilter(EntityId tractorBeamEmitterId);
        bool Query(EntityId tractorBeamEmitterId, FixVector2 target, out Node targetNode);

        void Select(VhId sourceId, EntityId tractorBeamEmitterId, EntityId nodeId);

        [Obsolete]
        void Deselect(VhId sourceId, EntityId tractorBeamEmitterId);
        void Deselect(VhId sourceId, EntityId tractorBeamEmitterId, SocketVhId? attachToSocketVhId);
    }
}
