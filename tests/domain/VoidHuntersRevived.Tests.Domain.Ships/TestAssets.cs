using Guppy.Core.Assets.Common;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Physics;
using VoidHuntersRevived.Domain.Physics.Common;

namespace VoidHuntersRevived.Tests.Domain.Ships
{
    public static class TestAssets
    {
        public static readonly Key<IEntityTemplate> TestSquareEntityTemplateKey = Key<IEntityTemplate>.GetByName(nameof(TestSquareEntityTemplateKey));

        public static class Colors
        {
            public static readonly Asset<Color> TestColor = new(AssetKey<Color>.Get(nameof(TestColor)), Color.White);
        }

        public static class BodyTemplates
        {
            public static readonly Asset<IBodyTemplate> TestSquareBodyTemplate = new(AssetKey<IBodyTemplate>.Get(nameof(TestSquareBodyTemplate)), new BodyTemplate()
            {
                Centeroid = new FixVector2(0.5, 0.5),
                Shapes = [
                    new Polygon(
                        density: (Fix64)0.5d,
                        vertices: [
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