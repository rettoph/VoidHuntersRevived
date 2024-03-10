using Autofac;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Serialization
{
    public abstract class ComponentSerializerFactory
    {
        public readonly Type Type;

        public ComponentSerializerFactory(Type type)
        {
            this.Type = type;
        }

        public abstract ComponentSerializer Create(ILifetimeScope scope);
    }

    public abstract class ComponentSerializerFactory<TComponent> : ComponentSerializerFactory
        where TComponent : unmanaged, IEntityComponent
    {
        public ComponentSerializerFactory() : base(typeof(TComponent))
        {
        }
    }

    public class ComponentSerializerFactory<TComponent, TSerializer> : ComponentSerializerFactory
        where TComponent : unmanaged, IEntityComponent
        where TSerializer : ComponentSerializer
    {
        public ComponentSerializerFactory() : base(typeof(TComponent))
        {
        }

        public override ComponentSerializer Create(ILifetimeScope scope)
        {
            return scope.Resolve<TSerializer>();
        }
    }
}
