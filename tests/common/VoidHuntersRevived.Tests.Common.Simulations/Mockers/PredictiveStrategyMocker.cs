using Guppy.Tests.Common.Mockers;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Strategies;
using VoidHuntersRevived.Domain.Simulations.Systems;
using VoidHuntersRevived.Tests.Common.Simulations.Mocks;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mockers
{
    public class PredictiveStrategyMocker : BaseStrategyMocker<PredictiveStrategy>
    {
        public ChannelMessageBusProxyMocker ChannelMessageBusProxyMocker { get; set; }
        public PredictiveStepServiceMocker PredictiveStepServiceMocker { get; set; }
        public PredictiveStepEventServiceMocker PredictiveStepEventServiceMocker { get; set; }

        public PredictiveStrategyMocker()
        {
            this.ChannelMessageBusProxyMocker = new ChannelMessageBusProxyMocker();

            this.PredictiveStepEventServiceMocker = new PredictiveStepEventServiceMocker
            {
                LoggerMocker = this.LoggerMocker,
                MessageBusMocker = this.ChannelMessageBusProxyMocker.MessageBusMocker
            };

            this.PredictiveStepServiceMocker = new PredictiveStepServiceMocker()
            {
                MessageBusMocker = this.ChannelMessageBusProxyMocker.MessageBusMocker
            };

            this.ChannelMessageBusProxyMocker.ProxyPublish<StepSequenceGroupEnum, Step>();

            this.SystemFactories.AddRange([
                x => new StepServiceUpdateSystem(this.PredictiveStepServiceMocker.PredictiveStepService),
                x => new PredictiveStepEventCleanSystem(this.PredictiveStepEventServiceMocker.PredictiveEventService)
            ]);
        }

        protected override PredictiveStrategy Build()
        {
            PredictiveStrategy strategy = new(
                this.GuppyScopeMocker.GetInstance(),
                this.PredictiveStepEventServiceMocker.PredictiveEventService,
                this.LoggerMocker.GetInstance());

            return strategy;
        }

        protected override void PostBuild()
        {
            base.PostBuild();

            // Manually configure message bus
            this.ChannelMessageBusProxyMocker.MessageBusMocker.GetInstance().SubscribeAll(this.Systems);
            this.ChannelMessageBusProxyMocker.MessageBusMocker.GetInstance().Subscribe(this.Strategy);
        }
    }
}
