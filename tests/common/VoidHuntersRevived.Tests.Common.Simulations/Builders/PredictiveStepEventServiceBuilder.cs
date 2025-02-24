using Guppy.Core.Logging.Common;
using Guppy.Core.Messaging.Common.Services;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Builders;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mocks
{
    public class PredictiveStepEventServiceBuilder : Builder<PredictiveStepEventService>
    {
        public required ChannelMessageBusBuilder ChannelMessageBusBuilder { get; init; }
        public required Mocker<ILogger> LoggerMocker { get; init; }

        protected override PredictiveStepEventService Build()
        {
            return new PredictiveStepEventService(
                this.ChannelMessageBusBuilder.Object,
                this.LoggerMocker.Object);
        }

        public static PredictiveStepEventServiceBuilder Create()
        {
            return new PredictiveStepEventServiceBuilder()
            {
                LoggerMocker = new Mocker<ILogger>(),
                ChannelMessageBusBuilder = new ChannelMessageBusBuilder()
                {
                    MessageBusServiceMocker = new Mocker<IMessageBusService>()
                }
            };
        }
    }
}
