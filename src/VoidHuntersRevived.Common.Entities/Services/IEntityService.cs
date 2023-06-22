using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEntityService
    {
        IdMap Create<TDescriptor>(EntityType<TDescriptor> type, VhId id)
            where TDescriptor : VoidHuntersEntityDescriptor, new();
        IdMap Create<TDescriptor>(EntityType<TDescriptor> type, VhId id, EntityInitializerDelegate initializer)
            where TDescriptor : VoidHuntersEntityDescriptor, new();

        IdMap GetIdMap(VhId id);
        IdMap GetIdMap(EGID egid);
        IdMap GetIdMap(uint entityId, ExclusiveGroupStruct groupId);

        IdMap Clone(VhId sourceId, VhId cloneId);

        void Destroy(VhId id);

        void Clean();
    }
}
