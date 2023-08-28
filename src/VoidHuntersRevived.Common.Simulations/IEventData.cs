using VoidHuntersRevived.Common.Entities.Enums;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface IEventData
    {
        VhId CalculateHash(in VhId source);
    }
}
