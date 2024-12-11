using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public readonly ref struct Entity(
        in uint index,
        in EntityLocalId localId,
        in EntityGlobalId globalId
    )
    {
        public readonly uint Index = index;
        public readonly EntityLocalId LocalId = localId;
        public readonly EntityGlobalId GlobalId = globalId;

        public EntityId EntityId => new(this.LocalId.Value, this.GlobalId.Value);
        public ExclusiveGroupStruct Group => this.LocalId.Value.groupID;
        public GroupIndex GroupIndex => new(this.Group, this.Index);
    }

    public readonly ref struct Entity<T>(
        in Entity entity,
        ref T value
    )
        where T : unmanaged, IEntityComponent
    {
        public readonly uint Index = entity.Index;
        public readonly EntityLocalId LocalId = entity.LocalId;
        public readonly EntityGlobalId GlobalId = entity.GlobalId;
        public readonly ref T Value = ref value;

        public EntityId EntityId => new(this.LocalId.Value, this.GlobalId.Value);
        public ExclusiveGroupStruct Group => this.LocalId.Value.groupID;
        public GroupIndex GroupIndex => new(this.Group, this.Index);
    }
}
