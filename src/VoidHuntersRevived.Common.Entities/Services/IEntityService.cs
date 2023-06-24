using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEntityService
    {
        IdMap Create(EntityType type, VhId id);
        IdMap Create(EntityType type, VhId id, EntityInitializerDelegate initializer);

        IdMap GetIdMap(VhId id);
        IdMap GetIdMap(EGID egid);
        IdMap GetIdMap(uint entityId, ExclusiveGroupStruct groupId);

        void Destroy(VhId id);

        void Clean();

        EntityType GetEntityType(VhId entityVhId);
    }
}
