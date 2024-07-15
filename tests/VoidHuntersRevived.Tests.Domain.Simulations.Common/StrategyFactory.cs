using Autofac;
using Autofac.Extras.Moq;
using Guppy.Core.Common;
using Guppy.Core.Messaging.Common;
using Guppy.Core.Messaging.Common.Services;
using Guppy.Core.Network.Common;
using Guppy.Core.Resources.Common;
using Guppy.Core.Resources.Common.Services;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Extensions;
using Guppy.Tests.Common.Mocks;
using Moq;
using Serilog;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Predictive;
using VoidHuntersRevived.Domain.Simulations.Services;
using VoidHuntersRevived.Tests.Common;

namespace VoidHuntersRevived.Tests.Domain.Simulations.Common
{
    public static class StrategyFactory
    {
        private static readonly SettingValue<int> StepsPerTick = new SettingValue<int>(Settings.StepsPerTick, 3);
        private static readonly SettingValue<Fix64> StepInterval = new SettingValue<Fix64>(Settings.StepInterval, (Fix64)20 / (Fix64)1000);

        public static IStrategy BuildPredictive(ISimulation simulation, Func<ILifetimeScope, IEnumerable<IEngine>>? customEnginesFactory)
        {
            EntitiesSubmissionScheduler scheduler = new EntitiesSubmissionScheduler();
            EnginesRoot enginesRoot = new EnginesRoot(scheduler);

            AutoMock automock = AutoMockBuilder.Create().Register(builder =>
            {
                builder.RegisterInstance(scheduler);
                builder.RegisterInstance(enginesRoot);
            }).Build();

            IFiltered<IEngine> engines = new MockFiltered<IEngine>(customEnginesFactory?.Invoke(automock.Container) ?? Enumerable.Empty<IEngine>());
            IMock<IBrokerService> brokerService = MockBuilder<IBrokerService>.Create().Build();
            IMock<ILogger> logger = MockBuilder<ILogger>.Create().Build();

            IEngineService engineService = new EngineService(
                engines,
                brokerService.Object,
                new MockFiltered<IEngineProvider>().ToLazy<IFiltered<IEngineProvider>>(),
                enginesRoot,
                scheduler);

            return new PredictiveStrategy(
                simulation.ToLazy(),
                engineService.ToLazy(),
                logger.Object.ToLazy());
        }

        public static IStrategy BuildClientLockstep(ISimulation simulation, Func<ILifetimeScope, IEnumerable<IEngine>>? customEnginesFactory, TickBuffer? tickBuffer)
        {
            EntitiesSubmissionScheduler scheduler = new EntitiesSubmissionScheduler();
            EnginesRoot enginesRoot = new EnginesRoot(scheduler);

            AutoMock automock = AutoMockBuilder.Create().Register(builder =>
            {
                builder.RegisterInstance(scheduler);
                builder.RegisterInstance(enginesRoot);
            }).Build();

            IMock<INetScope<IStrategy>> netScope = MockBuilder<INetScope<IStrategy>>.Create().Build();
            tickBuffer ??= new TickBuffer();
            IMock<ISettingService> settingsService = MockBuilder<ISettingService>.Create()
                .Setup(settings => settings.GetValue<Fix64>(Settings.StepInterval), () => StepInterval)
                .Setup(settings => settings.GetValue<int>(Settings.StepsPerTick), () => StepsPerTick)
                .Build();
            IFiltered<IEngine> engines = new MockFiltered<IEngine>(customEnginesFactory?.Invoke(automock.Container) ?? Enumerable.Empty<IEngine>());
            IMock<IBrokerService> brokerService = MockBuilder<IBrokerService>.Create().Build();
            IMock<ILogger> logger = MockBuilder<ILogger>.Create().Build();

            IEngineService engineService = new EngineService(
                engines,
                brokerService.Object,
                new MockFiltered<IEngineProvider>().ToLazy<IFiltered<IEngineProvider>>(),
                enginesRoot,
                scheduler);

            return new LockstepStrategy_Client(
                netScope.Object,
                tickBuffer,
                settingsService.Object,
                simulation.ToLazy(),
                engineService.ToLazy(),
                logger.Object.ToLazy());
        }

        public static IStrategy BuildServerLockstep(ISimulation simulation, Func<ILifetimeScope, IEnumerable<IEngine>>? customEnginesFactory)
        {
            EntitiesSubmissionScheduler scheduler = new EntitiesSubmissionScheduler();
            EnginesRoot enginesRoot = new EnginesRoot(scheduler);

            AutoMock automock = AutoMockBuilder.Create().Register(builder =>
            {
                builder.RegisterInstance(scheduler);
                builder.RegisterInstance(enginesRoot);
            }).Build();

            IMock<IBus> bus = MockBuilder<IBus>.Create().Build();
            IMock<ISettingService> settingsService = MockBuilder<ISettingService>.Create()
                .Setup(settings => settings.GetValue<Fix64>(Settings.StepInterval), () => StepInterval)
                .Setup(settings => settings.GetValue<int>(Settings.StepsPerTick), () => StepsPerTick)
                .Build();
            IFiltered<IEngine> engines = new MockFiltered<IEngine>(customEnginesFactory?.Invoke(automock.Container) ?? Enumerable.Empty<IEngine>());
            IMock<IBrokerService> brokerService = MockBuilder<IBrokerService>.Create().Build();
            IMock<ILogger> logger = MockBuilder<ILogger>.Create().Build();

            IEngineService engineService = new EngineService(
                engines,
                brokerService.Object,
                new MockFiltered<IEngineProvider>().ToLazy<IFiltered<IEngineProvider>>(),
                enginesRoot,
                scheduler);

            return new LockstepStrategy_Server(
                bus.Object,
                settingsService.Object,
                simulation.ToLazy(),
                engineService.ToLazy(),
                logger.Object.ToLazy());
        }
    }
}
