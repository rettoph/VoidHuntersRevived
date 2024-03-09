using Microsoft.Xna.Framework;
using Svelto.ECS;

namespace VoidHuntersRevived.Common.Pieces.Components
{
    public struct ColorPalette : IEntityComponent, IPieceComponent
    {
        public struct ColorPaletteValue
        {
            public Color Default;
            public Color Current;
        }

        public ColorPaletteValue Primary;
        public ColorPaletteValue Secondary;
    }
}
