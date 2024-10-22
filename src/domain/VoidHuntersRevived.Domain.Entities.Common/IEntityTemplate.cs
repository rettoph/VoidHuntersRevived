using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Common
{
    public interface IEntityTemplate
    {
        Key<IEntityTemplate> Key { get; }

        ComponentBuilderDictionary Components { get; }
        HashSet<Type> RequiredComponents { get; }
        EntityInitializerDelegate? Initializer { get; }

        IEntityTemplate WithComponent(IEntityComponent component);

        IEntityTemplate WithComponent<TComponent>(TComponent component)
            where TComponent : unmanaged, IEntityComponent;

        IEntityTemplate WithComponents(IEnumerable<IEntityComponent> components);

        IEntityTemplate RequireComponent(Type component);

        IEntityTemplate RequireComponent<TComponent>()
            where TComponent : unmanaged, IEntityComponent;

        IEntityTemplate RequireComponents(IEnumerable<Type> components);

        void Verify();
    }
}
