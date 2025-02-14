using VoidHuntersRevived.Domain.Simulations.Predictive;
using VoidHuntersRevived.Tests.Common.Simulations.Mocks;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mockers
{
    public class PredictiveStrategyMocker : BaseStrategyMocker<PredictiveStrategy>
    {
        public PredictiveStepEventServiceMocker PredictiveStepEventServiceMocker { get; set; }

        public PredictiveStrategyMocker()
        {
            this.PredictiveStepEventServiceMocker = new PredictiveStepEventServiceMocker
            {
                LoggerMocker = this.LoggerMocker
            };
        }

        protected override PredictiveStrategy Build()
        {
            return new PredictiveStrategy(
                this.GuppyScopeMocker.GetInstance(),
                this.PredictiveStepEventServiceMocker.PredictiveEventService,
                this.LoggerMocker.GetInstance());
        }
    }
}
