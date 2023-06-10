using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Systems;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Physics.Systems;
using VoidHuntersRevived.Common.ECS.Systems;

namespace VoidHuntersRevived.Domain.Physics.Systems
{
    internal sealed class SpaceSystem : PhysicsSystem, IUpdateSystem
    {
        public void Update(GameTime gameTime)
        {
            this.Space.Step(gameTime.ElapsedGameTime);
        }
    }
}
