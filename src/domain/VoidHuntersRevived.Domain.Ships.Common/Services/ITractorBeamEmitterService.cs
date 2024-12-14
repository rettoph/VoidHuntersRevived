using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Components;

namespace VoidHuntersRevived.Domain.Ships.Common.Services
{
    public interface ITractorBeamEmitterService
    {
        ref EntityFilterCollection GetTractorableFilter(EntityLocalId tractorBeamEmitterLocalId);
        bool Query(in Entity<TractorBeamEmitter> tractorBeamEmitter, FixVector2 target, out Node targetNode);

        void Select(VhId sourceId, in Entity<TractorBeamEmitter> tractorBeamEmitter, in Entity<Node> node);

        void Deselect(VhId sourceId, in Entity<TractorBeamEmitter> tractorBeamEmitter, SocketVhId? attachToSocketVhId);
    }
}
