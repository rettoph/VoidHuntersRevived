using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Components;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public delegate void EntityInitializerDelegate(ref EntityInitializer initializer);

    public interface IEntityService
    {
        IdMap Create(EntityName name, VhId id);
        IdMap Create(EntityName name, VhId id, EntityInitializerDelegate initializer);

        T GetProperty<T>(Property<T> id) 
            where T : class, IEntityProperty;

        bool TryGetProperty<T>(EGID id, out T property)
            where T : class, IEntityProperty;

        IdMap GetIdMap(VhId id);
        IdMap GetIdMap(EGID egid);
        IdMap GetIdMap(uint entityId, ExclusiveGroupStruct groupId);

        void Destroy(VhId id);
    }
}
