using Guppy.Attributes;
using Guppy.Resources;
using Guppy.Resources.Loaders;
using Guppy.Resources.Providers;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Constants;

namespace VoidHuntersRevived.Domain.Loaders
{
    [AutoLoad]
    internal sealed class SettingLoader : ISettingLoader
    {
        public IEnumerable<Setting> GetSettings(ISettingProvider settings)
        {
            yield return Settings.StepInterval;
            yield return Settings.StepsPerTick;
        }

        public void Load(ISettingProvider settings)
        {
            settings.Register(Settings.StepInterval, (Fix64)20 / (Fix64)1000);
            settings.Register(Settings.StepsPerTick, 3);
        }
    }
}
