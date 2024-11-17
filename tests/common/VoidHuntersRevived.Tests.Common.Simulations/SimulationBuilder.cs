using Guppy.Core.Common.Providers;
using Guppy.Core.Resources.Common;
using Guppy.Core.Resources.Common.Services;
using Guppy.Tests.Common;
using Moq;
using Serilog;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Engines;
using VoidHuntersRevived.Domain.Providers;
using VoidHuntersRevived.Domain.Simulations;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Tests.Common.Simulations.Services;
using VoidHuntersRevived.Tests.Registration.Entities.Extensions;

namespace VoidHuntersRevived.Tests.Common.Simulations
{
    public class SimulationBuilder : BaseInstanceBuilder<SimulationMocker>
    {
        private readonly List<Func<ServiceProviderMocker, IStrategyMocker>> _strategies = [];
        private readonly List<Action<ServiceCollectionMocker>> _configurations = [];

        public VhId Id;

        public SimulationBuilder(
            VhId id,
            SettingValue<Fix64> stepInterval,
            SettingValue<int> stepsPerTick,
            IEnumerable<EntityTemplateFragment> entityTemplateFragments,
            Action<ServiceCollectionMocker>? configuration = null)
        {
            this.Id = id;

            this.AddConfiguration(services =>
            {
                services.RegisterMocker<ISettingService>()
                    .Setup(settings => settings.GetValue(Settings.StepInterval), () => stepInterval)
                    .Setup(settings => settings.GetValue(Settings.StepsPerTick), () => stepsPerTick);

                services.RegisterMocker<ILoggerProvider>()
                    .Setup(loggers => loggers.Get(It.IsAny<Type>()), () => new Mocker<ILogger>().GetInstance());

                UniqueNumberProvider uniqueNumberProvider = new();
                EntitiesSubmissionScheduler entitiesSubmissionScheduler = new();
                EnginesRoot enginesRoot = new(entitiesSubmissionScheduler);
                TickBuffer tickBuffer = new();
                EngineServiceBuilder enginesServiceBuilder = new();
                EntitySubmissionEngine entitySubmissionEngine = new(entitiesSubmissionScheduler);

                services.RegisterFactory<UniqueNumberProvider>(provider => new());

                services.RegisterFactory<EntitiesSubmissionScheduler>(provider => new());

                services.RegisterFactory<EnginesRoot>(provider => new(
                    provider.Get<EntitiesSubmissionScheduler>()));

                services.RegisterFactory<TickBuffer>(provider => new());

                services.RegisterBuilder(new EngineServiceBuilder());

                services.RegisterFactory<EntitySubmissionEngine>(provider => new(
                    provider.Get<EntitiesSubmissionScheduler>()));

                services.RegisterEntityServices(entityTemplateFragments);

                configuration?.Invoke(services);
            });
        }

        public SimulationBuilder AddStrategy<TStrategy>(Func<ServiceProviderMocker, TStrategy> builder)
            where TStrategy : IStrategy
        {
            _strategies.Add(services => new StrategyMocker<TStrategy>(builder(services), services));

            return this;
        }

        public SimulationBuilder AddConfiguration(Action<ServiceCollectionMocker> configuration)
        {
            _configurations.Add(configuration);

            return this;
        }

        protected override SimulationMocker build()
        {
            IStrategyMocker[] strategies = _strategies.Select(builder =>
            {
                var services = new ServiceCollectionMocker();

                foreach (var conf in _configurations)
                    conf(services);

                ServiceProviderMocker provider = services.Build();

                return builder(provider);
            }).ToArray();

            SimulationMocker simulation = new(
                instance: new Simulation(this.Id, strategies.Select(x => x.Instance).ToArray()),
                strategies: strategies);

            return simulation;
        }
    }
}
