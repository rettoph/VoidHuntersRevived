using Guppy.Core.Assets.Common.Services;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Mockers;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mockers
{
    public class DefaultLockstepStepServiceBuilder : Builder<DefaultLockstepStepService>
    {
        public required Mocker<ISettingService> SettingServiceMocker { get; init; }
        public required DefaultLockstepTickServiceBuilder DefaultLockstepTickServiceBuilder { get; init; }
        public required ChannelMessageBusProxyMocker ChannelMessageBusProxyMocker { get; init; }

        protected override DefaultLockstepStepService Build()
        {
            return new DefaultLockstepStepService(
                settingService: this.SettingServiceMocker.Object,
                tickService: this.DefaultLockstepTickServiceBuilder.Object,
                messageBus: this.ChannelMessageBusProxyMocker.Object);
        }
    }
}
