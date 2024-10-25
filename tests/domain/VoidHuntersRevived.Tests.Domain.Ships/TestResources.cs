using Guppy.Core.Resources.Common;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Physics;
using VoidHuntersRevived.Domain.Physics.Common;

namespace VoidHuntersRevived.Tests.Domain.Ships
{
    public static class TestResources
    {
        public static class Colors
        {
            public static readonly Resource<Color> TestColor = new(ResourceKey<Color>.Get(nameof(TestColor)), Color.White);
        }

        public static class BodyTemplates
        {
            public static readonly Resource<IBodyTemplate> TestSquareBodyTemplate = new(ResourceKey<IBodyTemplate>.Get(nameof(TestSquareBodyTemplate)), new BodyTemplate()
            {
                Centeroid = new FixVector2(0.5, 0.5),
                Shapes = [
                    new Polygon(
                        (Fix64)0.5d,
                        [
                                new FixVector2(0, 0),
                                new FixVector2(1, 0),
                                new FixVector2(1, 1),
                                new FixVector2(0, 1)
                        ]
                    )
                ]
            });
        }
    }
}
