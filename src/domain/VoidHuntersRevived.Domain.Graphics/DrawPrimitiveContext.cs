using Guppy.Game.MonoGame.Common.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Graphics.Common;

namespace VoidHuntersRevived.Domain.Graphics
{
    public class DrawPrimitiveContext : IDrawPrimitiveContext
    {
        public required Camera2D Camera { get; init; }

        public GameTime GameTime { get; set; } = default!;

        public required EntitiesDB EntitiesDb { get; init; }
    }
}
