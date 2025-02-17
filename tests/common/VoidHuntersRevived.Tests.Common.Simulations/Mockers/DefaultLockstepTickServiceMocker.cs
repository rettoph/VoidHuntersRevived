using Guppy.Core.Resources.Common.Services;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Extensions;
using Guppy.Tests.Common.Mocks;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Tests.Common.Simulations.Mocks;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    public class DefaultLockstepTickServiceMocker : BaseMockerBuilder<DefaultLockstepTickService>
    {
        public Mocker<ISettingService> SettingServiceMocker { get; set; }

        public DefaultLockstepStepEventServiceMocker DefaultLockstepStepEventServiceMocker { get; set; }

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
                settingService: this.SettingServiceMocker.GetInstance());
        }

        public static DefaultLockstepTickServiceMocker Create(int stepsPerTick)
        {
            DefaultLockstepTickServiceMocker mocker = new();

            mocker.SettingServiceMocker.Setup(Settings.StepsPerTick, stepsPerTick);

            return mocker;
        }
    }
}
