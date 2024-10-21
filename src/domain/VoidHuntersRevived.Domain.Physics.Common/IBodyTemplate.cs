using VoidHuntersRevived.Common.FixedPoint;

namespace VoidHuntersRevived.Domain.Physics.Common
{
    public interface IBodyTemplate
    {
        FixVector2 Centeroid { get; }
        Polygon[] Shapes { get; }
    }
}
