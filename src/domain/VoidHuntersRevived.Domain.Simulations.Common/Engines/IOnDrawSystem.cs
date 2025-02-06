using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Systems;
using Microsoft.Xna.Framework;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Common.Systems
{
    public interface IOnDrawSystem : ISceneSystem, IEngine
    {
        [RequireSequenceGroup<OnDrawSequenceGroupEnum>]
        void OnDraw(GameTime gameTime);
    }
}