using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public readonly ref struct Entity(
        uint index,
        EntityLocalId localId,
        EntityGlobalId globalId
    )
    {
        public readonly uint Index = index;
        public readonly EntityLocalId LocalId = localId;
        public readonly EntityGlobalId GlobalId = globalId;

        public ExclusiveGroupStruct Group => this.LocalId.Value.groupID;
        public GroupIndex GroupIndex => new(this.Group, this.Index);
    }

    public readonly ref struct Entity<T>(
        uint index,
        EntityLocalId localId,
        EntityGlobalId globalId,
        ref T component
    )
        where T : unmanaged, IEntityComponent
    {
        public readonly uint Index = index;
        public readonly EntityLocalId LocalId = localId;
        public readonly EntityGlobalId GlobalId = globalId;
        public readonly ref T Component = ref component;

        public ExclusiveGroupStruct Group => this.LocalId.Value.groupID;
        public GroupIndex GroupIndex => new(this.Group, this.Index);

        public Entity(in Entity entity, ref T component) : this(entity.Index, entity.LocalId, entity.GlobalId, ref component)
        {

        }
    }
}
