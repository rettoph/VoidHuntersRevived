using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Components;

namespace VoidHuntersRevived.Common.Entities.Descriptors
{
    internal class StaticEntityDescriptor : IEntityDescriptor
    {
        private readonly IComponentBuilder[] _componentsToBuild = new IComponentBuilder[]
        {
            new ComponentBuilder<StaticData>(),
            new ComponentBuilder<Id<VoidHuntersEntityDescriptor>>(),
            new ComponentBuilder<Id<IEntityType>>()
        };

        public IComponentBuilder[] componentsToBuild => _componentsToBuild;
    }
}
