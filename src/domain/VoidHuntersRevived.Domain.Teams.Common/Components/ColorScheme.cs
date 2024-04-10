using Guppy.Resources;
using Guppy.Resources.Attributes;
using Microsoft.Xna.Framework;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Teams.Common.Components
{
    [PolymorphicJsonType<IEntityComponent>(nameof(ColorScheme))]
    public struct ColorScheme : IEntityComponent
    {
        public readonly Resource<Color> Primary;
        public readonly Resource<Color> Secondary;

        public ColorScheme(Resource<Color> primary, Resource<Color> secondary)
        {
            this.Primary = primary;
            this.Secondary = secondary;
        }
    }
}
