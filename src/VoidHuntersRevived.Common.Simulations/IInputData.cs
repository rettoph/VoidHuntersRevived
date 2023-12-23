namespace VoidHuntersRevived.Common.Simulations
{
    public interface IInputData : IEventData
    {
        VhId ShipVhId { get; }
    }
}
