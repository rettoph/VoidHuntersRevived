using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Components;

namespace VoidHuntersRevived.Common.Entities.Descriptors
{
    public class BaseEntityDescriptor : IEntityDescriptor
    {
        private readonly IComponentBuilder[] _componentsToBuild = new IComponentBuilder[]
        {
            new ComponentBuilder<EntityStatus>(),
            new ComponentBuilder<EntityId>(),
            new ComponentBuilder<Id<VoidHuntersEntityDescriptor>>(),
            new ComponentBuilder<Id<ITeam>>(),
            new ComponentBuilder<Id<IEntityType>>()
        };

        public IComponentBuilder[] componentsToBuild => _componentsToBuild;
    }
}
