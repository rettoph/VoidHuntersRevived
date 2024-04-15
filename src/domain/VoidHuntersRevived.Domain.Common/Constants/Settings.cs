using Guppy.Core.Resources;
using VoidHuntersRevived.Common.FixedPoint;

namespace VoidHuntersRevived.Domain.Common.Constants
{
    public static class Settings
    {
        public static readonly Setting<Fix64> StepInterval = Setting<Fix64>.Get(nameof(StepInterval), "Simulation step interval in seconds.", (Fix64)20 / (Fix64)1000);
        public static readonly Setting<int> StepsPerTick = Setting<int>.Get(nameof(StepsPerTick), "Number of steps taken each simulation tick.", 3);
    }
}
