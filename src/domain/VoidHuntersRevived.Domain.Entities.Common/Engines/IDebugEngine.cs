using Microsoft.Xna.Framework;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Engines
{
    public interface IDebugEngine : IEngine
    {
        string? Group => string.Empty;

        void RenderDebugInfo(GameTime gameTime);
    }
}
