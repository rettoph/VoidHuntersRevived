using Moq;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Tests.Common.Simulations.Mockers;

namespace VoidHuntersRevived.Tests.Domain.Simulations.Extensions
{
    public static class SimulationMockerExtensions
    {
        public static void PublishThenUpdateThenVerifyPrediction<TEvent>(
            this SimulationMocker simulationMocker,
            bool verified,
            Func<Times> publishTimes,
            Func<Times> revertTimes
        )
            where TEvent : IStepEvent, new()
        {
            TimeSpan simulatedRealtimeInterval = TimeSpan.FromMilliseconds(16);
            PredictiveStrategyMocker predictiveStrategyMocker = simulationMocker.SimulationMockers.OfType<PredictiveStrategyMocker>().Single();

            // Publish
            simulationMocker.Publish<TEvent>(verified);

            // Verify publish
            predictiveStrategyMocker.PredictiveStepEventServiceBuilder.MessageBusMocker.Verify(
                x => x.Publish<EventSequenceGroupEnum, VhId, TEvent>(It.Ref<VhId>.IsAny, It.Ref<TEvent>.IsAny),
                publishTimes);

            // Update
            simulationMocker.Update(simulatedRealtimeInterval, 1000);

            // Verify revert
            predictiveStrategyMocker.PredictiveStepEventServiceBuilder.MessageBusMocker.Verify(
                x => x.Publish<RevertEventSequenceGroupEnum, VhId, TEvent>(It.Ref<VhId>.IsAny, It.Ref<TEvent>.IsAny),
                revertTimes);
        }
    }
}
