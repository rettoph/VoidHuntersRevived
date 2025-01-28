using Autofac.Extras.Moq;
using Guppy.Core.Common;
using Guppy.Core.Logging.Common;
using Guppy.Core.Logging.Common.Services;
using Guppy.Core.Resources.Common;
using Guppy.Core.Resources.Common.Services;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Extensions;
using Moq;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Extensions;
using VoidHuntersRevived.Domain.Extensions;
using VoidHuntersRevived.Domain.Simulations;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Extensions;
using VoidHuntersRevived.Tests.Common.Entities.Services;

namespace VoidHuntersRevived.Tests.Common.Simulations
{
    public class SimulationBuilder : GuppyScopeMocker<SimulationBuilder, SimulationMocker>
    {
        private readonly List<Func<IGuppyScope, IStrategyMocker>> _strategies = [];

        public VhId Id;

        public SimulationBuilder(
            VhId id,
            SettingValue<Fix64> stepInterval,
            SettingValue<int> stepsPerTick,
            IEnumerable<EntityTemplateFragment> entityTemplateFragments)
        {
            this.Id = id;

            this.Register(builder =>
            {
                builder
                    .RegisterDomainCoreServices()
                    .RegisterDomainEntityServices()
                    .RegisterDomainSimulationServices();

                builder.RegisterMock<ISettingService>();
                builder.RegisterMock<ILogLevelService>();
                builder.RegisterMock<ILoggerService>();

                EntityTemplateFragmentServiceMocker templateFragmentService = new();
                templateFragmentService.AddFragments(entityTemplateFragments);
                builder.RegisterMocker(templateFragmentService);
            });

            this.Mock(mocker =>
            {
                mocker.Mocker<ISettingService>()
                    .Setup(settings => settings.GetValue(Settings.StepInterval), () => stepInterval)
                    .Setup(settings => settings.GetValue(Settings.StepsPerTick), () => stepsPerTick);


                mocker.Mocker<ILoggerService>()
                    .Setup(loggers => loggers.GetLogger(It.IsAny<Type>()), () => new Mocker<ILogger>().GetInstance())
                    .Setup(loggers => loggers.GetLogger<It.IsAnyType>(), new InvocationFunc(invocation =>
                    {
                        Type loggerContext = invocation.Method.ReturnType.GenericTypeArguments[0];
                        return Mocker.GetGenericInstance(typeof(ILogger<>), loggerContext);
                    }));
            });
        }

        public SimulationBuilder AddStrategy<TStrategy>()
            where TStrategy : IStrategy
        {
            this._strategies.Add(scope => new StrategyMocker<TStrategy>(scope));

            return this;
        }

        public override SimulationMocker Build()
        {
            IStrategyMocker[] strategies = this._strategies.Select(factory => factory(this.scope)).ToArray();

            SimulationMocker simulation = new(
                instance: new Simulation(this.Id, strategies.Select(x => x.Instance).ToArray()),
                strategies: strategies);

            return simulation;
        }
    }
}