using Guppy.Core.Messaging.Common;
using Guppy.Core.Resources.Common.Services;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Mocks;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mockers
{
    public class DefaultLockstepStepServiceMocker : BaseMockerBuilder<DefaultLockstepStepService>
    {
        public Mocker<ISettingService> SettingServiceMocker { get; set; } = new Mocker<ISettingService>();
        public DefaultLockstepTickServiceMocker DefaultLockstepTickServiceMocker { get; set; } = new DefaultLockstepTickServiceMocker();
        public Mocker<IMessageBus> MessageBusMocker { get; set; } = new Mocker<IMessageBus>();

        protected override DefaultLockstepStepService Build()
        {
            return new DefaultLockstepStepService(
                settingService: this.SettingServiceMocker.GetInstance(),
                tickService: this.DefaultLockstepTickServiceMocker.DefaultLockstepTickService,
                messageBus: this.MessageBusMocker.GetInstance());
        }
    }
}
