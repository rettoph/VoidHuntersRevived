using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEntityService
    {
        IdMap Create(EntityType type, VhId id);
        IdMap Create(EntityType type, VhId id, EntityInitializerDelegate initializer);

        IdMap GetIdMap(VhId vhid);
        IdMap GetIdMap(EGID egid);
        IdMap GetIdMap(uint entityId, ExclusiveGroupStruct groupId);

        bool TryGetIdMap(VhId vhid, out IdMap id);
        bool TryGetIdMap(EGID egid, out IdMap id);
        bool TryGetIdMap(uint entityId, ExclusiveGroupStruct groupId, out IdMap id);

        void Destroy(VhId id);

        void Clean();

        EntityType GetEntityType(VhId entityVhId);
    }
}
