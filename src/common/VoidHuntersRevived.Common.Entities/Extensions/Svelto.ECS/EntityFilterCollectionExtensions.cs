using VoidHuntersRevived.Common.Entities;

namespace Svelto.ECS
{
    public static class EntityFilterCollectionExtensions
    {
        public static void Add(this EntityFilterCollection filter, uint nativeId, in GroupIndex groupIndex)
        {
            filter.Add(nativeId, groupIndex.GroupID, groupIndex.Index);
        }

        public static void Add(this EntityFilterCollection filter, in EntityId id, in GroupIndex groupIndex)
        {
            filter.Add(id.EGID.entityID, groupIndex.GroupID, groupIndex.Index);
        }

        public static void Remove(this EntityFilterCollection filter, in EntityId id)
        {
            filter.Remove(id.EGID);
        }
    }
}
