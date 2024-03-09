using Microsoft.Xna.Framework;
using Svelto.ECS;

namespace VoidHuntersRevived.Common.Pieces.Components
{
    public struct ColorPalette : IEntityComponent, IPieceComponent
    {
        public struct ColorPaletteValue
        {
            public readonly Color Default;
            public Color Current;

            public ColorPaletteValue(Color @default, Color current)
            {
                Default = @default;
                Current = current;
            }
        }

        public readonly ColorPaletteValue Primary;
        public readonly ColorPaletteValue Secondary;

        public ColorPalette(Color primary, Color secondary)
        {
            Primary = new ColorPaletteValue(primary, primary);
            Secondary = new ColorPaletteValue(secondary, secondary);
        }
    }
}
