using Guppy.Core.Resources.Common;
using Guppy.Core.Serialization.Common.Attributes;
using Microsoft.Xna.Framework;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Teams.Common.Components
{
    [PolymorphicJsonType<IEntityComponent>(nameof(ColorScheme))]
    public struct ColorScheme : IEntityComponent
    {
        public readonly ResourceValue<Color> Primary;
        public readonly ResourceValue<Color> Secondary;

        public ColorScheme(ResourceValue<Color> primary, ResourceValue<Color> secondary)
        {
            this.Primary = primary;
            this.Secondary = secondary;
        }
    }
}
