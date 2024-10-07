using Guppy.Core.Resources.Common;
using Guppy.Core.Serialization.Common.Attributes;
using Microsoft.Xna.Framework;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Teams.Common.Components
{
    [PolymorphicJsonType<IEntityComponent>(nameof(ColorScheme))]
    public struct ColorScheme(ResourceValue<Color> primary, ResourceValue<Color> secondary) : IEntityComponent
    {
        public readonly ResourceValue<Color> Primary = primary;
        public readonly ResourceValue<Color> Secondary = secondary;
    }
}
