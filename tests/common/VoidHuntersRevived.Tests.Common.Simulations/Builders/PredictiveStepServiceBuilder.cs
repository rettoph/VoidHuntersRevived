using Guppy.Tests.Common;
using Guppy.Tests.Common.Builders;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mockers
{
    public class PredictiveStepServiceBuilder : Builder<PredictiveStepService>
    {
        public required ChannelMessageBusBuilder ChannelMessageBusBuilder { get; init; }

        protected override PredictiveStepService Build()
        {
            return new PredictiveStepService(
                this.ChannelMessageBusBuilder.Object);
        }
    }
}
