using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Components;

namespace VoidHuntersRevived.Domain.Entities.Common.Descriptors
{
    public class InstanceEntityDescriptor : IEntityDescriptor
    {
        private readonly IComponentBuilder[] _componentsToBuild = new IComponentBuilder[]
        {
            new ComponentBuilder<InstanceEntity>(),
            new ComponentBuilder<EntityStatus>(),
            new ComponentBuilder<EntityId>(),
            new ComponentBuilder<Id<VoidHuntersEntityDescriptor>>(),
            new ComponentBuilder<Id<ITeam>>(),
            new ComponentBuilder<Id<IEntityType>>(),
        };

        public IComponentBuilder[] componentsToBuild => _componentsToBuild;
    }
}
