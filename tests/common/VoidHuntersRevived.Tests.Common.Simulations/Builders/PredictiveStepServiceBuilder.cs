using Guppy.Tests.Common;
using Guppy.Tests.Common.Mockers;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mockers
{
    public class PredictiveStepServiceBuilder : Builder<PredictiveStepService>
    {
        public required ChannelMessageBusProxyMocker ChannelMessageBusProxyMocker { get; init; }

        protected override PredictiveStepService Build()
        {
            return new PredictiveStepService(
                this.ChannelMessageBusProxyMocker.Object);
        }
    }
}
