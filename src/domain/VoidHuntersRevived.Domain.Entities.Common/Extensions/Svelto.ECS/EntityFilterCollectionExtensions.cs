using VoidHuntersRevived.Domain.Entities.Common;

namespace Svelto.ECS
{
    public static class EntityFilterCollectionExtensions
    {
        public static void Add(this EntityFilterCollection filter, uint nativeId, in GroupIndex groupIndex)
        {
            filter.Add(nativeId, groupIndex.GroupID, groupIndex.Index);
        }

        public static void Add(this EntityFilterCollection filter, in EntityLocalId id, in GroupIndex groupIndex)
        {
            filter.Add(id.Value.entityID, groupIndex.GroupID, groupIndex.Index);
        }

        public static void Add(this EntityFilterCollection filter, in EntityLocalId id, in uint index)
        {
            filter.Add(id.Value.entityID, id.Value.groupID, index);
        }

        public static void Add<T>(this EntityFilterCollection filter, in Entity<T> entity)
            where T : unmanaged, IEntityComponent
        {
            filter.Add(entity.LocalId.Value.entityID, entity.Group, entity.Index);
        }

        public static void Remove(this EntityFilterCollection filter, in EntityLocalId id)
        {
            filter.Remove(id.Value);
        }
    }
}