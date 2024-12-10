using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public readonly ref struct Entity<T>(
        uint index,
        LocalEntityId localId,
        GlobalEntityId globalId,
        ref T value
    )
        where T : unmanaged, IEntityComponent
    {
        public readonly uint Index = index;
        public readonly LocalEntityId LocalId = localId;
        public readonly GlobalEntityId GlobalId = globalId;
        public readonly ref T Value = ref value;

        public EntityId EntityId => new(this.LocalId.Value, this.GlobalId.Value);
        public ExclusiveGroupStruct Group => this.LocalId.Value.groupID;
        public GroupIndex GroupIndex => new(this.Group, this.Index);
    }
}
