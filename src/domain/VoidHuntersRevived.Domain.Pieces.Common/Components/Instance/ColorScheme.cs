using Microsoft.Xna.Framework;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Pieces.Common.Components.Instance
{
    public struct ColorScheme : IEntityComponent, IPieceComponent
    {
        public struct ColorSchemeValue
        {
            public readonly Color Default;
            public Color Current;

            public ColorSchemeValue(Color @default, Color current)
            {
                Default = @default;
                Current = current;
            }
        }

        public readonly ColorSchemeValue Primary;
        public readonly ColorSchemeValue Secondary;

        public ColorScheme(Color primary, Color secondary)
        {
            Primary = new ColorSchemeValue(primary, primary);
            Secondary = new ColorSchemeValue(secondary, secondary);
        }
    }
}
