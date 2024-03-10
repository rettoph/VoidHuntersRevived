using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Components;

namespace VoidHuntersRevived.Domain.Entities.Common.Descriptors
{
    internal class StaticEntityDescriptor : IEntityDescriptor
    {
        private readonly IComponentBuilder[] _componentsToBuild = new IComponentBuilder[]
        {
            new ComponentBuilder<StaticEntity>(),
            new ComponentBuilder<Id<VoidHuntersEntityDescriptor>>(),
            new ComponentBuilder<Id<IEntityType>>()
        };

        public IComponentBuilder[] componentsToBuild => _componentsToBuild;
    }
}
