using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Ships.Common.Services
{
    public interface ITacticalService
    {
        void AddUse(EntityId tacticalId);
        void RemoveUse(EntityId tacticalId);
    }
}
