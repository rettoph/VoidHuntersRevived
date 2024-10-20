using Guppy.Core.Network.Common.Enums;
using Guppy.Tests.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Predictive;

namespace VoidHuntersRevived.Tests.Common.Simulations.Strategies
{
    public sealed class PredictiveStrategyBuilder : BaseStrategyBuilder
    {
        public PredictiveStrategyBuilder() : base(PeerType.Client, StrategyTypeEnum.Predictive)
        {
        }

        protected override IStrategy build(ISimulation simulation)
        {
            return new PredictiveStrategy(
                simulation.ToLazy(),
                this.EngineServiceBuilder.GetLazy<IEngineService>(),
                this.Logger.GetLazy());
        }
    }
}
