using Guppy.Core.Logging.Common;
using Guppy.Core.Messaging.Common.Services;
using Guppy.Core.Resources.Common.Services;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Builders;
using Guppy.Tests.Common.Extensions;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Tests.Common.Simulations.Mocks;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    public class DefaultLockstepTickServiceBuilder : Builder<DefaultLockstepTickService>
    {
        public required Mocker<ISettingService> SettingServiceMocker { get; init; }

        public required DefaultLockstepStepEventServiceBuilder DefaultLockstepStepEventServiceMocker { get; init; }

        protected override DefaultLockstepTickService Build()
        {
            return new DefaultLockstepTickService(
                eventService: this.DefaultLockstepStepEventServiceMocker.Object,
                settingService: this.SettingServiceMocker.Object);
        }

        public static DefaultLockstepTickServiceBuilder Create(int stepsPerTick)
        {
            DefaultLockstepTickServiceBuilder mocker = new()
            {
                DefaultLockstepStepEventServiceMocker = new DefaultLockstepStepEventServiceBuilder()
                {
                    ChannelMessageBusBuilder = new ChannelMessageBusBuilder()
                    {
                        MessageBusServiceMocker = new Mocker<IMessageBusService>()
                    },
                    LoggerMocker = new Mocker<ILogger>()
                },
                SettingServiceMocker = new Mocker<ISettingService>().SetupReturn(Settings.StepsPerTick, stepsPerTick)
            };

            return mocker;
        }
    }
}
