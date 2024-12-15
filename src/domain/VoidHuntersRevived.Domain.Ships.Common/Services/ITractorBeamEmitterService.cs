using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;

namespace VoidHuntersRevived.Domain.Ships.Common.Services
{
    public interface ITractorBeamEmitterService
    {
        ref EntityFilterCollection GetTractorableFilter(EntityLocalId tractorBeamEmitterLocalId);
        bool Query(EntityLocalId tractorBeamEmitterLocalId, FixVector2 target, out Node targetNode);

        void Select(VhId sourceId, EntityGlobalId tractorBeamEmitterGlobalId, EntityGlobalId nodeGlobalId);

        void Deselect(VhId sourceId, EntityGlobalId tractorBeamEmitterGlobalId, SocketVhId? attachToSocketVhId);
    }
}
