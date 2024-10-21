using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Physics.Common;

namespace VoidHuntersRevived.Domain.Physics
{
    public class BodyTemplate : IBodyTemplate
    {
        public required FixVector2 Centeroid { get; init; }

        public required Polygon[] Shapes { get; init; }
    }
}
