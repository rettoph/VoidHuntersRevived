using Guppy.Core.Resources.Common;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Tests.Common.Simulations.Extensions;

namespace VoidHuntersRevived.Tests.Common.Simulations
{
    public class SimulationBuilder : BaseInstanceBuilder<SimulationMocker>
    {
        private readonly List<Func<ServiceProviderMocker, IStrategyMocker>> _strategies = [];
        private readonly List<Action<ServiceProviderMocker>> _configurations = [];

        public VhId Id;

        public SimulationBuilder(
            VhId id,
            SettingValue<Fix64> stepInterval,
            SettingValue<int> stepsPerTick,
            IEnumerable<EntityTemplateFragment> entityTemplateFragments,
            Func<IEnumerable<IEngine>> engines)
        {
            this.Id = id;

            this.AddCoreConfigurations(
                stepInterval,
                stepsPerTick,
                entityTemplateFragments,
                engines);
        }

        public SimulationBuilder AddStrategy<TStrategy>(Func<ServiceProviderMocker, TStrategy> builder)
            where TStrategy : IStrategy
        {
            _strategies.Add(services => new StrategyMocker<TStrategy>(builder(services), services));

            return this;
        }

        public SimulationBuilder AddConfiguration(Action<ServiceProviderMocker> configuration)
        {
            _configurations.Add(configuration);

            return this;
        }

        protected override SimulationMocker build()
        {
            IStrategyMocker[] strategies = _strategies.Select(builder =>
            {
                var services = new ServiceProviderMocker();

                foreach (var conf in _configurations)
                    conf(services);

                return builder(services);
            }).ToArray();

            SimulationMocker simulation = new(
                instance: new Simulation(this.Id, strategies.Select(x => x.Instance).ToArray()),
                strategies: strategies);

            return simulation;
        }
    }
}
