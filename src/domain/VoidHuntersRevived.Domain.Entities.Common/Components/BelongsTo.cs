using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public struct BelongsTo : IEntityComponent
    {
        public EntityId ParentId { get; set; }
    }
}
