using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Components;

namespace VoidHuntersRevived.Domain.Entities.Common.Descriptors
{
    public class InstanceEntityDescriptor : IEntityDescriptor
    {
        private readonly IComponentBuilder[] _componentsToBuild = new IComponentBuilder[]
        {
            new ComponentBuilder<EntityId>(),
            new ComponentBuilder<EntityStatus>(),
            new ComponentBuilder<InstanceEntity>(),
            new ComponentBuilder<BelongsTo<TypeEntity, InstanceEntity>>()
        };

        public IComponentBuilder[] componentsToBuild => _componentsToBuild;
    }
}
