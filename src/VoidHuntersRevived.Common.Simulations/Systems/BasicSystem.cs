﻿namespace VoidHuntersRevived.Common.Simulations.Systems
{
    public abstract class BasicSystem : ISimulationSystem
    {
        public ISimulation Simulation { get; private set; } = null!;

        public virtual void Initialize(ISimulation simulation)
        {
            this.Simulation = simulation;
        }

        public virtual void Dispose()
        {
        }
    }
}
