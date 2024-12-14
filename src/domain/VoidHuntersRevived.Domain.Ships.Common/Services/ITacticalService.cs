using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Ships.Common.Services
{
    public interface ITacticalService
    {
        void AddUse(EntityLocalId tacticalId);
        void RemoveUse(EntityLocalId tacticalId);
    }
}
