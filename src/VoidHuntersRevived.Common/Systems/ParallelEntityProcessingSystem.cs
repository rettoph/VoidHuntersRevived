using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Components;
using VoidHuntersRevived.Common.Services;

namespace VoidHuntersRevived.Common.Systems
{
    public abstract class ParallelEntityProcessingSystem : ParallelEntitySystem, IUpdateSimulationSystem
    {
        protected ParallelEntityProcessingSystem(ISimulationService simulations, AspectBuilder aspectBuilder) : base(simulations, aspectBuilder)
        {
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
