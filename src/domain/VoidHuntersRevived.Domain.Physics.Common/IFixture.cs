using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Physics.Common
{
    public interface IFixture
    {
        VhId Id { get; }

        EntityId EntityId { get; }

        FixVector2 Centeroid { get; }

        IBody Body { get; }
    }
}
