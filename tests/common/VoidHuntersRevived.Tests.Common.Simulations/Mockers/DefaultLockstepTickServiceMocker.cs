using Guppy.Core.Resources.Common.Services;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Extensions;
using Guppy.Tests.Common.Mocks;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Tests.Common.Simulations.Mocks;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    public class DefaultLockstepTickServiceMocker : MockBuilder<DefaultLockstepTickService>
    {
        public Mocker<ISettingService> SettingServiceMocker { get; init; }

        public DefaultLockstepStepEventServiceMocker DefaultLockstepStepEventServiceMocker { get; init; }

        public DefaultLockstepTickService DefaultLockstepTickService => this.GetInstance();

        public DefaultLockstepTickServiceMocker()
        {
            this.DefaultLockstepStepEventServiceMocker = new DefaultLockstepStepEventServiceMocker();
            this.SettingServiceMocker = new Mocker<ISettingService>();
        }

        protected override DefaultLockstepTickService Build()
        {
            return new DefaultLockstepTickService(
                eventService: this.DefaultLockstepStepEventServiceMocker.DefaultLockstepStepEventService,
                settingService: this.SettingServiceMocker.Object);
        }

        public static DefaultLockstepTickServiceMocker Create(int stepsPerTick)
        {
            DefaultLockstepTickServiceMocker mocker = new();

            mocker.SettingServiceMocker.SetupReturn(Settings.StepsPerTick, stepsPerTick);

            return mocker;
        }
    }
}
