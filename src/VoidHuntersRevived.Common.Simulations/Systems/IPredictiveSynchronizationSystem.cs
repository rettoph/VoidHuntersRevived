using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Simulations.Systems;

namespace VoidHuntersRevived.Common.Simulations.Systems
{
    public interface IPredictiveSynchronizationSystem : ISystem
    {
        void Synchronize(ISimulation lockstep, GameTime gameTime, Fix64 damping);
    }
}
