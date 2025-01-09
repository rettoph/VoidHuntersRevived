using Guppy.Core.Common.Attributes;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Common.Engines
{
    public interface IOnDrawEngine : IGraphicsEngine
    {
        [RequireSequenceGroup<OnDrawSequenceGroupEnum>]
        void OnDraw(GameTime gameTime);
    }
}