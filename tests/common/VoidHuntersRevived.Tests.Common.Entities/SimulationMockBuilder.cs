using Autofac.Extras.Moq;
using Guppy.Core.Common;
using Guppy.Core.Common.Enums;
using Guppy.Core.Logging.Common;
using Guppy.Core.Logging.Common.Services;
using Guppy.Core.Resources.Common;
using Guppy.Core.Resources.Common.Services;
using Guppy.Game.Extensions;
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
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;
using VoidHuntersRevived.Domain.Simulations.Extensions;
using VoidHuntersRevived.Tests.Common.Entities.Services;
using VoidHuntersRevived.Tests.Common.Simulations.Mocks;

namespace VoidHuntersRevived.Tests.Common.Simulations
{
    public class SimulationMockBuilder : GuppyScopeMocker<SimulationMockBuilder, SimulationMock>
    {
        private readonly List<Func<IGuppyScope, ISimulation, IStrategyAutoMock>> _strategies = [];

        public VhId Id;

        public SimulationMockBuilder(
            VhId id,
            SettingValue<Fix64> stepInterval,
            SettingValue<int> stepsPerTick,
            IEnumerable<EntityTemplateFragment> entityTemplateFragments) : base(GuppyScopeTypeEnum.Root, [])
        {
            this.Id = id;

            this.Register(builder =>
            {
                builder
                    .RegisterCommonGameServices()
                    .RegisterDomainCoreServices()
                    .RegisterDomainEntityServices()
                    .RegisterDomainSimulationServices();

                builder.RegisterMock<ISettingService>().SingleInstance();
                builder.RegisterMock<ILogLevelService>().SingleInstance();
                builder.RegisterMock<ILoggerService>().SingleInstance();

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

        public SimulationMockBuilder AddStrategy<TStrategy>()
            where TStrategy : class, IStrategy
        {
            this._strategies.Add((scope, simulation) => new StrategyMock<TStrategy>(scope, simulation));

            return this;
        }

        public override SimulationMock Build()
        {
            List<IStrategyAutoMock> strategies = [];

            Simulation instance = new(this.Id, simulation =>
            {
                foreach (var strategyMockerFactory in this._strategies)
                {
                    IStrategyAutoMock strategyMocker = strategyMockerFactory(this.scope, simulation);
                    strategies.Add(strategyMocker);
                }

                return strategies.Select(x => x.Instance);
            });

            instance.Initialize();

            SimulationMock simulation = new(
                instance: instance,
                strategies: strategies.ToArray());

            return simulation;
        }
    }
}