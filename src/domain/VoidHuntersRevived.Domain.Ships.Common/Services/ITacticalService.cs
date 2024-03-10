using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Ships.Services
{
    public interface ITacticalService
    {
        void AddUse(EntityId tacticalId);
        void RemoveUse(EntityId tacticalId);
    }
}
