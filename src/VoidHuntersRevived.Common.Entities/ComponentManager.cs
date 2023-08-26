using Autofac;
using Guppy.Common;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Serialization;

namespace VoidHuntersRevived.Common.Entities
{
    public abstract class ComponentManager
    {
        public readonly Type Type;
        public readonly IComponentBuilder Builder;
        public readonly ComponentSerializerFactory SerializerFactory;

        internal ComponentManager(IComponentBuilder builder, ComponentSerializerFactory serializerFactory)
        {
            this.SerializerFactory = serializerFactory;
            this.Builder = builder;
            this.Type = this.Builder.GetEntityComponentType();

            ThrowIf.Type.IsNotAssignableFrom(this.SerializerFactory.Type, this.Builder.GetEntityComponentType());
        }
    }

    public sealed class ComponentManager<TComponent, TSerializer> : ComponentManager
        where TComponent : unmanaged, IEntityComponent
        where TSerializer : ComponentSerializer<TComponent>
    {
        public ComponentManager(IComponentBuilder builder) : base(builder, new ComponentSerializerFactory<TComponent, TSerializer>())
        {
        }
        public ComponentManager() : this(new ComponentBuilder<TComponent>())
        {

        }
        public ComponentManager(in TComponent initializer) : this(new ComponentBuilder<TComponent>(in initializer))
        {

        }
    }

}
