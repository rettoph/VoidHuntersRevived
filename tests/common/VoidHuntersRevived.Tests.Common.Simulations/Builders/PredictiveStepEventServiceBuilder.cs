using Guppy.Core.Logging.Common;
using Guppy.Core.Messaging.Common.Services;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Mockers;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mocks
{
    public class PredictiveStepEventServiceBuilder : Builder<PredictiveStepEventService>
    {
        public required ChannelMessageBusProxyMocker ChannelMessageBusProxyMocker { get; init; }
        public required Mocker<ILogger> LoggerMocker { get; init; }

        protected override PredictiveStepEventService Build()
        {
            return new PredictiveStepEventService(
                this.ChannelMessageBusProxyMocker.Object,
                this.LoggerMocker.Object);
        }

        public static PredictiveStepEventServiceBuilder Create()
        {
            return new PredictiveStepEventServiceBuilder()
            {
                LoggerMocker = new Mocker<ILogger>(),
                ChannelMessageBusProxyMocker = new ChannelMessageBusProxyMocker()
                {
                    MessageBusServiceMocker = new Mocker<IMessageBusService>()
                }
            };
        }
    }
}
