using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEntityService
    {
        IdMap GetIdMap(VhId vhid);
        IdMap GetIdMap(EGID egid);
        IdMap GetIdMap(uint entityId, ExclusiveGroupStruct groupId);

        bool TryGetIdMap(VhId vhid, out IdMap id);
        bool TryGetIdMap(EGID egid, out IdMap id);
        bool TryGetIdMap(uint entityId, ExclusiveGroupStruct groupId, out IdMap id);

        EntityType GetEntityType(VhId entityVhId);

        void Clean();
    }
}
