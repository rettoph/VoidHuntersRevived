using VoidHuntersRevived.Common.Entities.Enums;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface IEventData
    {
        bool IsPredictable { get; }

        VhId CalculateHash(in VhId source);
    }
}
