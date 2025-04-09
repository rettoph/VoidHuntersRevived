using Guppy.Core.Assets.Common;
using Microsoft.Xna.Framework;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Teams.Common.Components
{
    public readonly struct ColorScheme(Asset<Color> primary, Asset<Color> secondary) : IEntityComponent
    {
        public readonly Asset<Color> Primary = primary;
        public readonly Asset<Color> Secondary = secondary;
    }
}