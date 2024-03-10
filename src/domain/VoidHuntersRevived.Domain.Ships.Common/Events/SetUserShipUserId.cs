using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Ships.Common.Events
{
    internal class SetUserShipUserId : IEventData
    {
        public bool IsPredictable => true;

        public VhId ShipVhId { get; init; }
        public int? UserId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<SetUserShipUserId, VhId, VhId, bool, int>.Instance.Calculate(source, this.ShipVhId, this.UserId.HasValue, this.UserId ?? 0);
        }
    }
}
