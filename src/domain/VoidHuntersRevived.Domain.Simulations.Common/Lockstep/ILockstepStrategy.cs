using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Simulations.Common.Lockstep
{
    public interface ILockstepStrategy : IStrategy
    {
        int StepsPerTick { get; }
        Fix64 StepInterval { get; }
        TimeSpan StepTimeSpan { get; }
        TimeSpan TimeSinceStep { get; }
        int StepsSinceTick { get; }
        Tick CurrentTick { get; }

        IEnumerable<Tick> History { get; }

        event OnEventDelegate<Id<IStepEvent>, IStepEvent>? OnEvent;
    }
}