using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Enums;
using Microsoft.Xna.Framework;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Simulations.Common.Engines
{
    public interface IOnDebugEngine : IEngine
    {
        [RequireSequenceGroup<DebugSequenceGroup>]
        void OnDebug(GameTime gameTime);
    }
}
