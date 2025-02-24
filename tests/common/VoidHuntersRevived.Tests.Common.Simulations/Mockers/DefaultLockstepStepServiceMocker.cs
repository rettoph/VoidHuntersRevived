using Guppy.Core.Messaging.Common;
using Guppy.Core.Resources.Common.Services;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Mocks;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mockers
{
    public class DefaultLockstepStepServiceMocker : MockBuilder<DefaultLockstepStepService>
    {
        public Mocker<ISettingService> SettingServiceMocker { get; init; } = new Mocker<ISettingService>();
        public DefaultLockstepTickServiceMocker DefaultLockstepTickServiceMocker { get; init; } = new DefaultLockstepTickServiceMocker();
        public Mocker<IMessageBus> MessageBusMocker { get; init; } = new Mocker<IMessageBus>();

        public DefaultLockstepStepService DefaultLockstepStepService => this.GetInstance();

        protected override DefaultLockstepStepService Build()
        {
            return new DefaultLockstepStepService(
                settingService: this.SettingServiceMocker.Object,
                tickService: this.DefaultLockstepTickServiceMocker.DefaultLockstepTickService,
                messageBus: this.MessageBusMocker.Object);
        }
    }
}
