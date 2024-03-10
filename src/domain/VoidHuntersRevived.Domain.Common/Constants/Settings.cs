using Guppy.Resources;
using VoidHuntersRevived.Common.FixedPoint;

namespace VoidHuntersRevived.Domain.Common.Constants
{
    public static class Settings
    {
        public static readonly Setting<Fix64> StepInterval = Setting.Get<Fix64>(nameof(StepInterval), (Fix64)20 / (Fix64)1000, "Simulation step interval in seconds.");
        public static readonly Setting<int> StepsPerTick = Setting.Get<int>(nameof(StepsPerTick), 3, "Number of steps taken each simulation tick.");
    }
}
