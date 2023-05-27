using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Common.Simulations.Systems
{
    public abstract class ParallelEntityProcessingSystem : ParallelEntitySystem, ISimulationUpdateSystem
    {
        protected ParallelEntityProcessingSystem(AspectBuilder aspectBuilder) : base(aspectBuilder)
        {
        }

        public virtual void Update(ISimulation simulation, GameTime gameTime)
        {
            foreach (ParallelKey entityKey in this.Entities[simulation.Type].ActiveEntities)
            {
                this.Process(simulation, gameTime, entityKey);
            }   
        }

        protected abstract void Process(ISimulation simulation, GameTime gameTime, ParallelKey entityKey);
    }
}
