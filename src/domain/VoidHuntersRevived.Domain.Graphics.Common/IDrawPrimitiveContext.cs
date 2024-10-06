using Guppy.Game.MonoGame.Common.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Graphics.Common
{
    public interface IDrawPrimitiveContext
    {
        Camera2D Camera { get; }
        GameTime GameTime { get; }
        EntitiesDB EntitiesDb { get; }
    }
}
