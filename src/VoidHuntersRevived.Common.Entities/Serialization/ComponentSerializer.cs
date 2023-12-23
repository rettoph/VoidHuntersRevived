using Guppy.Attributes;
using Guppy.Enums;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Options;

namespace VoidHuntersRevived.Common.Entities.Serialization
{
    public abstract class ComponentSerializer
    {
        public readonly Type Type;

        internal ComponentSerializer(Type type)
        {
            Type = type;
        }

        public abstract void Serialize(EntityWriter writer, in GroupIndex groupIndex, EntitiesDB entitiesDB, in SerializationOptions options);
        public abstract void Deserialize(in DeserializationOptions options, EntityReader reader, ref EntityInitializer initializer, in EntityId id);
    }

    [Service(ServiceLifetime.Scoped, null, true)]
    public abstract class ComponentSerializer<TComponent> : ComponentSerializer
        where TComponent : unmanaged, IEntityComponent
    {
        public ComponentSerializer() : base(typeof(TComponent))
        {
        }

        public override void Serialize(EntityWriter writer, in GroupIndex groupIndex, EntitiesDB entitiesDB, in SerializationOptions options)
        {
            var (components, _) = entitiesDB.QueryEntities<TComponent>(groupIndex.GroupID);
            ref var component = ref components[groupIndex.Index];

            this.Write(writer, component, in options);
        }
        public override void Deserialize(in DeserializationOptions options, EntityReader reader, ref EntityInitializer initializer, in EntityId id)
        {
            initializer.Init<TComponent>(this.Read(in options, reader, in id));
        }

        protected abstract void Write(EntityWriter writer, in TComponent instance, in SerializationOptions options);
        protected abstract TComponent Read(in DeserializationOptions options, EntityReader reader, in EntityId id);
    }

    public abstract class NotImplementedComponentSerializer<TComponent> : ComponentSerializer<TComponent>
        where TComponent : unmanaged, IEntityComponent
    {
        protected override TComponent Read(in DeserializationOptions options, EntityReader reader, in EntityId id)
        {
            throw new NotImplementedException();
        }

        protected override void Write(EntityWriter writer, in TComponent instance, in SerializationOptions options)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class RawComponentSerializer<TComponent> : ComponentSerializer<TComponent>
        where TComponent : unmanaged, IEntityComponent
    {
        protected unsafe override TComponent Read(in DeserializationOptions options, EntityReader reader, in EntityId id)
        {
            Span<byte> span = stackalloc byte[sizeof(TComponent)];
            reader.Read(span);
            fixed (byte* pBytes = &span[0])
            {
                TComponent* components = (TComponent*)&pBytes[0];
                return components[0];
            }
        }

        protected unsafe override void Write(EntityWriter writer, in TComponent instance, in SerializationOptions options)
        {
            TComponent bytes = instance;
            byte* pBytes = (byte*)&bytes;
            var span = new ReadOnlySpan<byte>(pBytes, sizeof(TComponent));

            writer.Write(span);
        }
    }

    public abstract class DoNotSerializeComponentSerializer<TComponent> : ComponentSerializer<TComponent>
        where TComponent : unmanaged, IEntityComponent
    {
        public DoNotSerializeComponentSerializer()
        {
        }

        public override void Serialize(EntityWriter writer, in GroupIndex groupIndex, EntitiesDB entitiesDB, in SerializationOptions options)
        {

        }
        public override void Deserialize(in DeserializationOptions options, EntityReader reader, ref EntityInitializer initializer, in EntityId id)
        {

        }

        protected override void Write(EntityWriter writer, in TComponent instance, in SerializationOptions options)
        {
            throw new NotImplementedException();
        }

        protected override TComponent Read(in DeserializationOptions options, EntityReader reader, in EntityId id)
        {
            throw new NotImplementedException();
        }
    }
}
