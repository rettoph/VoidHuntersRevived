namespace VoidHuntersRevived.Common.Simulations
{
    public interface IInputData : IEventData
    {
        Guid PilotId { get; }
    }
}
