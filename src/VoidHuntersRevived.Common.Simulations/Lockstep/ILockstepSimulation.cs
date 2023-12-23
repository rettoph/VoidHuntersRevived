namespace VoidHuntersRevived.Common.Simulations.Lockstep
{
    public interface ILockstepSimulation : ISimulation
    {
        Tick CurrentTick { get; }

        IEnumerable<Tick> History { get; }

        event OnEventDelegate<EventDto>? OnEvent;
    }
}
