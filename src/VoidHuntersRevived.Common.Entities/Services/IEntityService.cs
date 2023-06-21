using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Components;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEntityService
    {
        IdMap Create<TDescriptor>(EntityType<TDescriptor> type, VhId id)
            where TDescriptor : IEntityDescriptor, new();
        IdMap Create<TDescriptor>(EntityType<TDescriptor> type, VhId id, EntityInitializerDelegate initializer)
            where TDescriptor : IEntityDescriptor, new();

        IdMap GetIdMap(VhId id);
        IdMap GetIdMap(EGID egid);
        IdMap GetIdMap(uint entityId, ExclusiveGroupStruct groupId);

        void Destroy<TDescriptor>(VhId id)
            where TDescriptor : IEntityDescriptor, new();

        void Clean();
    }
}
