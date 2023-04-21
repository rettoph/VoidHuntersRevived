using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Common.Simulations.Systems
{
    public abstract class ParallelEntityProcessingSystem : ParallelEntitySystem, ISimulationUpdateSystem
    {
        protected ParallelEntityProcessingSystem(ISimulationService simulations, AspectBuilder aspectBuilder) : base(simulations, aspectBuilder)
        {
        }

        public virtual void Initialize(ISimulation simulation)
        {
            //
        }

        public virtual void Update(ISimulation simulation, GameTime gameTime)
        {
            foreach (var entityId in this.Entities[simulation.Type].ActiveEntities)
            {
                this.Process(simulation, gameTime, entityId);
            }   
        }

        protected abstract void Process(ISimulation simulation, GameTime gameTime, int entityId);
    }
}
