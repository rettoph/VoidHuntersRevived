using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Components;

namespace VoidHuntersRevived.Domain.Entities.Common.Descriptors
{
    internal class TypeEntityDescriptor : IEntityDescriptor
    {
        private readonly IComponentBuilder[] _componentsToBuild = new IComponentBuilder[]
        {
            new ComponentBuilder<EntityId>(),
            new ComponentBuilder<TypeData>(),
            new ComponentBuilder<Id<VoidHuntersEntityDescriptor>>(),
            new ComponentBuilder<Id<IEntityType>>(),
            new ComponentBuilder<HasMany<InstanceData>>()
        };

        public IComponentBuilder[] componentsToBuild => _componentsToBuild;
    }
}
