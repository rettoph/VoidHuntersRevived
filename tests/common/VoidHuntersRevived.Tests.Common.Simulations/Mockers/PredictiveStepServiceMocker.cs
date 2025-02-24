using Guppy.Core.Messaging.Common;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Mocks;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mockers
{
    public class PredictiveStepServiceMocker : MockBuilder<PredictiveStepService>
    {
        public Mocker<IMessageBus> MessageBusMocker { get; init; } = new Mocker<IMessageBus>();
        public PredictiveStepService PredictiveStepService => this.GetInstance();

        protected override PredictiveStepService Build()
        {
            return new PredictiveStepService(
                this.MessageBusMocker.Object);
        }
    }
}
