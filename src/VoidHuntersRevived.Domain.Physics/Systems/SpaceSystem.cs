using VoidHuntersRevived.Common.Simulations.Systems;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Simulations.Systems;

namespace VoidHuntersRevived.Domain.Physics.Systems
{
    internal sealed class SpaceSystem : BasicSystem, IUpdateSystem
    {
        public void Update(GameTime gameTime)
        {
            this.Simulation.Space.Step(gameTime.ElapsedGameTime);
        }
    }
}
