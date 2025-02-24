using Guppy.Core.Messaging.Common.Services;
using Guppy.Core.Messaging.Systems.Scoped;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Builders;
using VoidHuntersRevived.Domain.Simulations.Strategies;
using VoidHuntersRevived.Domain.Simulations.Systems;
using VoidHuntersRevived.Tests.Common.Simulations.Mocks;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mockers
{
    public class PredictiveStrategyMocker : StrategyMocker<PredictiveStrategy>
    {
        public ChannelMessageBusBuilder ChannelMessageBusBuilder { get; }
        public PredictiveStepServiceBuilder PredictiveStepServiceBuilder { get; }
        public PredictiveStepEventServiceBuilder PredictiveStepEventServiceBuilder { get; }

        public PredictiveStrategyMocker()
        {
            this.ChannelMessageBusBuilder = new ChannelMessageBusBuilder()
            {
                MessageBusServiceMocker = new Mocker<IMessageBusService>()
            };

            this.PredictiveStepEventServiceBuilder = new PredictiveStepEventServiceBuilder
            {
                LoggerMocker = this.LoggerMocker,
                ChannelMessageBusBuilder = this.ChannelMessageBusBuilder
            };

            this.PredictiveStepServiceBuilder = new PredictiveStepServiceBuilder()
            {
                ChannelMessageBusBuilder = this.ChannelMessageBusBuilder
            };

            this.SystemFactories.AddRange([
                x => new StepServiceUpdateSystem(this.PredictiveStepServiceBuilder.Object),
                x => new StepEventServiceFlushSystem(this.PredictiveStepEventServiceBuilder.Object),
                x => new PredictiveStepEventCleanSystem(this.PredictiveStepEventServiceBuilder.Object),
                x => new AutoSubscribeScopedSystemsToBrokerServiceSystem(
                    messageBus: this.ChannelMessageBusBuilder.Object,
                    scopedSystemService: this.ScopedSystemServiceMocker.Object)
            ]);
        }

        protected override PredictiveStrategy Build()
        {
            PredictiveStrategy strategy = new(
                this.GuppyScopeMocker.Object,
                this.PredictiveStepEventServiceBuilder.Object,
                this.LoggerMocker.Object);

            return strategy;
        }
    }
}
