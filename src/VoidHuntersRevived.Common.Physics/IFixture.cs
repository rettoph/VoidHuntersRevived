using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Physics
{
    public interface IFixture
    {
        VhId Id { get; }

        EntityId EntityId { get; }

        FixVector2 Centeroid { get; }

        IBody Body { get; }
    }
}
