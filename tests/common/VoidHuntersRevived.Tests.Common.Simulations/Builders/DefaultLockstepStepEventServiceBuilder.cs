using Guppy.Core.Logging.Common;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Mockers;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mocks
{
    public class DefaultLockstepStepEventServiceBuilder : Builder<DefaultLockstepStepEventService>
    {
        public required ChannelMessageBusProxyMocker ChannelMessageBusProxyMocker { get; init; }
        public required Mocker<ILogger> LoggerMocker { get; init; }

        protected override DefaultLockstepStepEventService Build()
        {
            return new DefaultLockstepStepEventService(
                this.ChannelMessageBusProxyMocker.Object,
                this.LoggerMocker.Object);
        }
    }
}
