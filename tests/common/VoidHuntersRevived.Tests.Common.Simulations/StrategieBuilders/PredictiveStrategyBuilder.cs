using Guppy.Core.Network.Common.Enums;
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

        protected override IStrategy build()
        {
            return new PredictiveStrategy(
                this.EngineServiceBuilder.GetLazy<IEngineService>(),
                this.Logger.GetLazy());
        }
    }
}
