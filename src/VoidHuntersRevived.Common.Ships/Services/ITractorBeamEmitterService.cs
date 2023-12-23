using Svelto.ECS;
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

        [Obsolete]
        void Deselect(EntityId tractorBeamEmitterId);
        void Deselect(EntityId tractorBeamEmitterId, SocketVhId? attachToSocketVhId);
    }
}
