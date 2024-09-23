using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Engines
{
    public interface IDebugEngine : IEngine
    {
        string? Group => string.Empty;

        [RequireSequenceGroup<DrawDebugComponentSequenceGroup>]
        void RenderDebugInfo(GameTime gameTime);
    }
}
