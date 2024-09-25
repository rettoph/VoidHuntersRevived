using Guppy.Core.Common.Attributes;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Common.Engines
{
    public interface IOnDrawEngine : IEngine
    {
        [RequireSequenceGroup<OnDrawSequenceGroup>]
        void OnDraw(GameTime gameTime);
    }
}
