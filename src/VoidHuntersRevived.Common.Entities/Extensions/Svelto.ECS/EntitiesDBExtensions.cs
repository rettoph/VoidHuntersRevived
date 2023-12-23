namespace Svelto.ECS
{
    public static class EntitiesDBExtensions
    {
        public static EntityReference GetEntityReference(this EntitiesDB entitiesDB, uint entityId, ExclusiveGroupStruct groupId)
        {
            return entitiesDB.GetEntityReference(new EGID(entityId, groupId));
        }

        public static ref T QueryEntityByIndex<T>(this EntitiesDB entitiesDB, uint index, ExclusiveGroupStruct groupID)
            where T : unmanaged, IEntityComponent
        {
            var (entities, _) = entitiesDB.QueryEntities<T>(groupID);

            return ref entities[index];
        }

        public static bool TryGetEntityByIndex<T>(this EntitiesDB entitiesDB, uint index, ExclusiveGroupStruct groupID, out T entity)
            where T : unmanaged, IEntityComponent
        {
            if (!entitiesDB.HasAny<T>(groupID))
            {
                entity = default;
                return false;
            }

            var (entities, _) = entitiesDB.QueryEntities<T>(groupID);

            entity = entities[index];
            return true;
        }
    }
}
