using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IUserShipMappingService
    {
        void Add(int userId, EventId shipKey);

        void Remove(EventId shipKey);

        public int GetUserId(EventId shipKey);

        public EventId GetShipKey(int userId);

        public bool TryGetShipKey(int userId, out EventId shipKey);
    }
}
