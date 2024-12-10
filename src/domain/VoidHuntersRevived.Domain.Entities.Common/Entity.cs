using Svelto.DataStructures;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public readonly struct Entity<T>(
        ExclusiveGroupStruct group,
        uint index,
        NB<LocalEntityId> localEntityIds,
        NB<GlobalEntityId> globalEntityIds,
        NB<T> values
    )
        where T : unmanaged, IEntityComponent
    {
        private readonly NB<LocalEntityId> _localEntityIds = localEntityIds;
        private readonly NB<GlobalEntityId> _globalEntityIds = globalEntityIds;
        private readonly NB<T> _values = values;

        public readonly ExclusiveGroupStruct Group = group;
        public readonly uint Index = index;

        public LocalEntityId LocalId => _localEntityIds[this.Index];
        public GlobalEntityId GlobalId => _globalEntityIds[this.Index];
        public T Value => _values[this.Index];
        public GroupIndex GroupIndex => new(this.Group, this.Index);
    }
}
