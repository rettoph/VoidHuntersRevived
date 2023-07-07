using Autofac;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Game.Client
{
    public class LocalGameGuppy : GameGuppy
    {
        public LocalGameGuppy(ISimulationService simulations) : base(simulations)
        {
        }

        public override void Initialize(ILifetimeScope scope)
        {
            //this.Simulations.Configure(SimulationType.Lockstep | SimulationType.Predictive);
            this.Simulations.Configure(SimulationType.Lockstep);

            base.Initialize(scope);
        }
    }
}
