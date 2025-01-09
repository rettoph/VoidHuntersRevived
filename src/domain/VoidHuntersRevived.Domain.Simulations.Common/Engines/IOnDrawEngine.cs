using Guppy.Core.Common.Attributes;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Common.Engines
{
    public interface IOnDrawEngine : IGraphicsEngine
    {
        [RequireSequenceGroup<OnDrawSequenceGroup>]
        void OnDraw(GameTime gameTime);
    }
}