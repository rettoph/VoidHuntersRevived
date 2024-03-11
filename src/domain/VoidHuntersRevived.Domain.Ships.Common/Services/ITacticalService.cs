using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Ships.Common.Services
{
    public interface ITacticalService
    {
        void AddUse(EntityId tacticalId);
        void RemoveUse(EntityId tacticalId);
    }
}
