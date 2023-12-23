using Microsoft.Xna.Framework;
using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities.Engines
{
    public interface IDebugEngine : IEngine
    {
        string? Group => string.Empty;

        void RenderDebugInfo(GameTime gameTime);
    }
}
