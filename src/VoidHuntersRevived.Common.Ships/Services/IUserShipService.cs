using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Ships.Services
{
    public interface IUserShipService
    {
        void SetUserId(VhId sourceId, EntityId shipId, int? userId);

        bool TryGetShipId(int userId, out EntityId shipId);
        bool TryGetUserId(EntityId shipId, out int userId);
        bool TryGetUserId(VhId shipVhId, out int userId);
        bool TryGetCurrentUserShipId(out EntityId shipId);

        bool TryGetShipId(int? userId, out EntityId shipId)
        {
            if (userId is not null)
            {
                return this.TryGetShipId(userId.Value, out shipId);
            }

            shipId = default;
            return false;
        }
    }
}
