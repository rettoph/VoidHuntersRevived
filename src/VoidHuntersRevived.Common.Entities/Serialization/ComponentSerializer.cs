using Guppy.Attributes;
using Guppy.Enums;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities.Serialization
{
    public abstract class ComponentSerializer
    {
        public readonly Type Type;

        internal ComponentSerializer(Type type)
        {
            Type = type;
        }

        public abstract void Serialize(EntityWriter writer, in GroupIndex groupIndex, EntitiesDB entitiesDB);
        public abstract void Deserialize(EntityReader reader, ref EntityInitializer initializer, in EntityId id);
    }

    [Service(ServiceLifetime.Scoped, null, true)]
    public abstract class ComponentSerializer<TComponent> : ComponentSerializer
        where TComponent : unmanaged, IEntityComponent
    {
        public ComponentSerializer() : base(typeof(TComponent))
        {
        }

        public override void Serialize(EntityWriter writer, in GroupIndex groupIndex, EntitiesDB entitiesDB)
        {
            var (components, _) = entitiesDB.QueryEntities<TComponent>(groupIndex.GroupID);
            ref var component = ref components[groupIndex.Index];

            this.Write(writer, component);
        }
        public override void Deserialize(EntityReader reader, ref EntityInitializer initializer, in EntityId id)
        {
            initializer.Init<TComponent>(this.Read(reader, id));
        }

        protected abstract void Write(EntityWriter writer, TComponent instance);
        protected abstract TComponent Read(EntityReader reader, EntityId id);
    }

    public abstract class NotImplementedComponentSerializer<TComponent> : ComponentSerializer<TComponent>
        where TComponent : unmanaged, IEntityComponent
    {
        protected override TComponent Read(EntityReader reader, EntityId id)
        {
            throw new NotImplementedException();
        }

        protected override void Write(EntityWriter writer, TComponent instance)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class RawComponentSerializer<TComponent> : ComponentSerializer<TComponent>
        where TComponent : unmanaged, IEntityComponent
    {
        protected unsafe override TComponent Read(EntityReader reader, EntityId id)
        {
            Span<byte> span = stackalloc byte[sizeof(TComponent)];
            reader.Read(span);
            fixed (byte* pBytes = &span[0])
            {
                TComponent* components = (TComponent*)&pBytes[0];
                return components[0];
            }
        }

        protected unsafe override void Write(EntityWriter writer, TComponent instance)
        {
            byte* pBytes = (byte*)&instance;
            var span = new ReadOnlySpan<byte>(pBytes, sizeof(TComponent));

            writer.Write(span);
        }
    }
}
