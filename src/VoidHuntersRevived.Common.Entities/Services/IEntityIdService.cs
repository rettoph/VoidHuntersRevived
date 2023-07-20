using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEntityIdService
    {
        EntityId GetId(VhId vhid);
        EntityId GetId(EGID egid);
        EntityId GetId(uint entityId, ExclusiveGroupStruct groupId);

        bool TryGetId(VhId vhid, out EntityId id);
        bool TryGetId(EGID egid, out EntityId id);
        bool TryGetId(uint entityId, ExclusiveGroupStruct groupId, out EntityId id);

        IEntityType GetEntityType(VhId entityVhId);

        void Clean();
    }
}
