using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public readonly ref struct InitializingEntity(
        in EntityLocalId localId,
        in EntityGlobalId globalId,
        ref EntityInitializer initializer,
        in IEntityTemplate template
    )
    {
        public readonly EntityLocalId LocalId = localId;
        public readonly EntityGlobalId GlobalId = globalId;
        public readonly EntityInitializer Initializer = initializer;
        public readonly IEntityTemplate Template = template;

        public EntityId EntityId => new(this.LocalId.Value, this.GlobalId.Value);
    }
}
