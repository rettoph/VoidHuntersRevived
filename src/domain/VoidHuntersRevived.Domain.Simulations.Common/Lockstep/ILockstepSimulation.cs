namespace VoidHuntersRevived.Domain.Simulations.Common.Lockstep
{
    public interface ILockstepSimulation : ISimulation
    {
        Tick CurrentTick { get; }

        IEnumerable<Tick> History { get; }

        event OnEventDelegate<EventDto>? OnEvent;
    }
}
