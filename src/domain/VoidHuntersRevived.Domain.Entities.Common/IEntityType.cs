using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public interface IEntityType
    {
        IKey<IEntityType> Key { get; }

        ComponentBuilderDictionary Components { get; }
        HashSet<Type> RequiredComponents { get; }

        IEntityType WithComponent(IEntityComponent component);

        IEntityType WithComponent<TComponent>(TComponent component)
            where TComponent : unmanaged, IEntityComponent;

        IEntityType WithComponents(IEnumerable<IEntityComponent> components);

        IEntityType RequireComponent(Type component);

        IEntityType RequireComponent<TComponent>()
            where TComponent : unmanaged, IEntityComponent;

        IEntityType RequireComponents(IEnumerable<Type> components);
    }
}
