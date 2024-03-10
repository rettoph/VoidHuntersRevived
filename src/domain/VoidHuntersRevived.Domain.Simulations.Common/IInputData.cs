using VoidHuntersRevived.Common.Core;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface IInputData : IEventData
    {
        VhId ShipVhId { get; }
    }
}
