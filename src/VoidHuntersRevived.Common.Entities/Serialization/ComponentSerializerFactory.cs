using Autofac;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Serialization
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

    internal class DefaultComponentSerializerFactory<TComponent> : ComponentSerializerFactory<TComponent>
        where TComponent : unmanaged, IEntityComponent
    {
        private readonly ComponentSerializer _serializer;

        public DefaultComponentSerializerFactory(DefaultComponentSerializer<TComponent> serializer)
        {
            _serializer = serializer;
        }

        public override ComponentSerializer Create(ILifetimeScope scope)
        {
            return _serializer;
        }
    }
}
