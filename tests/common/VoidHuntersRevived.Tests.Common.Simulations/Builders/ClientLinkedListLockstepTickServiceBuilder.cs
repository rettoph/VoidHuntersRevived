using Guppy.Core.Assets.Common.Services;
using Guppy.Tests.Common;
using Guppy.Tests.Common.Extensions;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Tests.Common.Simulations.Mockers
{
    public class ClientLinkedListLockstepTickServiceBuilder : Builder<ClientLinkedListLockstepTickService>
    {
        public required Mocker<ISettingService> SettingServiceMocker { get; init; }

        protected override ClientLinkedListLockstepTickService Build()
        {
            return new ClientLinkedListLockstepTickService(
                settingService: this.SettingServiceMocker.Object);
        }

        public static ClientLinkedListLockstepTickServiceBuilder Create(int stepsPerTick)
        {
            return new ClientLinkedListLockstepTickServiceBuilder()
            {
                SettingServiceMocker = new Mocker<ISettingService>()
                    .SetupReturn(x => x.GetValue(Settings.StepsPerTick), Settings.StepsPerTick.MockValue(stepsPerTick))
            };
        }
    }
}
