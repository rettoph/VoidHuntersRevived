using System.Collections.ObjectModel;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Factories;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    internal sealed partial class SimulationService : ISimulationService, IDisposable
    {
        private readonly IStrategiesFactory _strategiesFactory;
        private readonly List<ISimulation> _simulations;

        public ReadOnlyCollection<ISimulation> Instances { get; }

        public SimulationService(IStrategiesFactory strategiesFactory)
        {
            _strategiesFactory = strategiesFactory;
            _simulations = new List<ISimulation>();

            this.Instances = new ReadOnlyCollection<ISimulation>(_simulations);
        }

        public void Dispose()
        {
            foreach (ISimulation simulation in _simulations)
            {
                simulation.Dispose();
            }
        }

        public ISimulation Create(VhId id, params StrategyTypeEnum[] strategies)
        {
            ISimulation simulation = new Simulation(id, _strategiesFactory, strategies);

            _simulations.Add(simulation);

            return simulation;
        }
    }
}
