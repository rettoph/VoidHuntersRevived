using Guppy.Core.Messaging.Common;
using Guppy.Core.Network.Common;
using Guppy.Core.Resources.Common.Services;
using Serilog;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Predictive;
using VoidHuntersRevived.Tests.Common.Simulations;

namespace VoidHuntersRevived.Tests.Registration.Simulations.Extensions
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
    }
}
