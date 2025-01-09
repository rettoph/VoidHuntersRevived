using Svelto.DataStructures;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public readonly struct GroupEntities<T>
        where T : unmanaged, IEntityComponent
    {
        public readonly ExclusiveGroupStruct Group;

        public readonly NB<EntityLocalId> LocalEntityIds;
        public readonly NB<EntityGlobalId> GlobalEntityIds;
        public readonly NB<T> Values;
    }
}