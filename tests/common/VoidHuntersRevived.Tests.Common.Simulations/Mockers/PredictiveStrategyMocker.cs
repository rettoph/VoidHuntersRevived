using Guppy.Core.Messaging.Common.Services;
using Guppy.Core.Messaging.Systems.Scoped;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Mockers;
using VoidHuntersRevived.Domain.Simulations.Strategies;
using VoidHuntersRevived.Domain.Simulations.Systems;
using VoidHuntersRevived.Tests.Common.Simulations.Mocks;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mockers
{
    public class PredictiveStrategyMocker : StrategyMocker<PredictiveStrategy>
    {
        public ChannelMessageBusProxyMocker ChannelMessageBusProxyMocker { get; }
        public PredictiveStepServiceBuilder PredictiveStepServiceBuilder { get; }
        public PredictiveStepEventServiceBuilder PredictiveStepEventServiceBuilder { get; }

        public PredictiveStrategyMocker()
        {
            this.ChannelMessageBusProxyMocker = new ChannelMessageBusProxyMocker()
            {
                MessageBusServiceMocker = new Mocker<IMessageBusService>()
            };

            this.PredictiveStepEventServiceBuilder = new PredictiveStepEventServiceBuilder
            {
                LoggerMocker = this.LoggerMocker,
                ChannelMessageBusProxyMocker = this.ChannelMessageBusProxyMocker
            };

            this.PredictiveStepServiceBuilder = new PredictiveStepServiceBuilder()
            {
                ChannelMessageBusProxyMocker = this.ChannelMessageBusProxyMocker
            };

            this.SystemFactories.AddRange([
                x => new StepServiceUpdateSystem(this.PredictiveStepServiceBuilder.Object),
                x => new StepEventServiceFlushSystem(this.PredictiveStepEventServiceBuilder.Object),
                x => new PredictiveStepEventCleanSystem(this.PredictiveStepEventServiceBuilder.Object),
                x => new AutoSubscribeScopedSystemsToBrokerServiceSystem(
                    messageBus: this.ChannelMessageBusProxyMocker.Object,
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
