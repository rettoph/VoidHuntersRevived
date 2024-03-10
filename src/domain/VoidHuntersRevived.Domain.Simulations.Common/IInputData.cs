using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Simulations.Common
{
    public interface IInputData : IEventData
    {
        VhId ShipVhId { get; }
    }
}
