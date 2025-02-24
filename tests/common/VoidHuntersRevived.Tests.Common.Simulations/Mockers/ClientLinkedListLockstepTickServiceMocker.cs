using Guppy.Core.Resources.Common.Services;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Extensions;
using Guppy.Tests.Common.Mocks;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mockers
{
    public class ClientLinkedListLockstepTickServiceMocker : MockBuilder<ClientLinkedListLockstepTickService>
    {
        public Mocker<ISettingService> SettingServiceMocker { get; }

        public ClientLinkedListLockstepTickService ClientLinkedListLockstepTickService => this.GetInstance();

        public ClientLinkedListLockstepTickServiceMocker(int stepsPerTick)
        {
            this.SettingServiceMocker = new Mocker<ISettingService>()
                .SetupReturn(x => x.GetValue(Settings.StepsPerTick), Settings.StepsPerTick.MockValue(stepsPerTick));
        }

        protected override ClientLinkedListLockstepTickService Build()
        {
            return new ClientLinkedListLockstepTickService(
                settingService: this.SettingServiceMocker.Object);
        }

        public static ClientLinkedListLockstepTickServiceMocker Create(int stepsPerTick)
        {
            return new ClientLinkedListLockstepTickServiceMocker(stepsPerTick);
        }
    }
}
