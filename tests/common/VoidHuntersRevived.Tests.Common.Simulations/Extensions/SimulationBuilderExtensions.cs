using Guppy.Core.Messaging.Common;
using Guppy.Core.Network.Common;
using Guppy.Core.Resources.Common;
using Guppy.Core.Resources.Common.Services;
using Serilog;
using Svelto.ECS;
using Svelto.ECS.Schedulers;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Engines;
using VoidHuntersRevived.Domain.Providers;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Predictive;
using VoidHuntersRevived.Tests.Common.Entities.Extensions;
using VoidHuntersRevived.Tests.Common.Simulations.Services;

namespace VoidHuntersRevived.Tests.Common.Simulations.Extensions
{
    public static class SimulationBuilderExtensions
    {
        public static SimulationBuilder AddPredictiveStrategy(this SimulationBuilder builder)
        {
            return builder.AddStrategy(services =>
            {
                return new PredictiveStrategy(
                    services.GetLazy<IEngineService>(),
                    services.GetLazy<ILogger>());
            });
        }

        public static SimulationBuilder AddLockstepClientStrategy(this SimulationBuilder builder)
        {
            return builder.AddStrategy(services =>
            {
                return new LockstepStrategy_Client(
                    services.Get<INetScope<IStrategy>>(),
                    services.Get<TickBuffer>(),
                    services.Get<ISettingService>(),
                    services.GetLazy<IEngineService>(),
                    services.GetLazy<ILogger>());
            });
        }

        public static SimulationBuilder AddLockstepServerStrategy(this SimulationBuilder builder)
        {
            return builder.AddStrategy(services =>
            {
                return new LockstepStrategy_Server(
                    services.Get<IBus>(),
                    services.Get<ISettingService>(),
                    services.GetLazy<IEngineService>(),
                    services.GetLazy<ILogger>());
            });
        }

        internal static SimulationBuilder AddCoreConfigurations(
            this SimulationBuilder builder,
            SettingValue<Fix64> stepInterval,
            SettingValue<int> stepsPerTick,
            IEnumerable<EntityTemplateFragment> entityTemplateFragments,
            Func<IEnumerable<IEngine>> engines)
        {
            return builder.AddConfiguration(services =>
            {
                services.GetMocker<ISettingService>()
                    .Setup(settings => settings.GetValue(Settings.StepInterval), () => stepInterval)
                    .Setup(settings => settings.GetValue(Settings.StepsPerTick), () => stepsPerTick);

                UniqueNumberProvider uniqueNumberProvider = new();
                EntitiesSubmissionScheduler entitiesSubmissionScheduler = new();
                EnginesRoot enginesRoot = new(entitiesSubmissionScheduler);
                TickBuffer tickBuffer = new();
                EngineServiceBuilder enginesServiceBuilder = new();
                EntitySubmissionEngine entitySubmissionEngine = new(entitiesSubmissionScheduler);

                services.AddRange([
                    uniqueNumberProvider,
                    entitiesSubmissionScheduler,
                    enginesRoot,
                    tickBuffer,
                    enginesServiceBuilder,
                    entitySubmissionEngine
                ]);

                services.AddRange(engines());

                services.AddEntityServices(entityTemplateFragments);
            });
        }
    }
}
