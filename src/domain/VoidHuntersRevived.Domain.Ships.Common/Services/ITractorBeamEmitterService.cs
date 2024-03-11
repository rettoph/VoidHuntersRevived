using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;

namespace VoidHuntersRevived.Domain.Ships.Common.Services
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
