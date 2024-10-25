using Guppy.Core.Resources.Common;
using Guppy.Core.Serialization.Common.Attributes;
using Microsoft.Xna.Framework;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Teams.Common.Components
{
    [PolymorphicJsonType<IEntityComponent>(nameof(ColorScheme))]
    public struct ColorScheme(Resource<Color> primary, Resource<Color> secondary) : IEntityComponent
    {
        public readonly Resource<Color> Primary = primary;
        public readonly Resource<Color> Secondary = secondary;
    }
}
