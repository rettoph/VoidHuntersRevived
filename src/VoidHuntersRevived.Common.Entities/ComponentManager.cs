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
        public readonly ComponentSerializer Serializer;

        internal ComponentManager(IComponentBuilder builder, ComponentSerializer serializer)
        {
            this.Serializer = serializer;
            this.Builder = builder;
            this.Type = this.Builder.GetEntityComponentType();

            ThrowIf.Type.IsNotAssignableFrom(this.Serializer.Type, this.Builder.GetEntityComponentType());
        }
    }

    public sealed class ComponentManager<TComponent> : ComponentManager
        where TComponent : unmanaged, IEntityComponent
    {
        public ComponentManager(ComponentBuilder<TComponent> builder, ComponentSerializer<TComponent> serializer) : base(builder, serializer)
        {

        }
    }
}
