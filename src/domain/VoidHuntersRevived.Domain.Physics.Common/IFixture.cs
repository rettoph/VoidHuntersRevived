using VoidHuntersRevived.Common.FixedPoint;

namespace VoidHuntersRevived.Domain.Physics.Common
{
    public interface IFixture
    {
        FixtureId Id { get; }

        FixVector2 Centeroid { get; }

        IBody Body { get; }
    }
}
