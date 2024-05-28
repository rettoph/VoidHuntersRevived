using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public struct BelongsTo<TParent> : IEntityComponent
    {
        public EntityId ParentId { get; set; }
    }
}
