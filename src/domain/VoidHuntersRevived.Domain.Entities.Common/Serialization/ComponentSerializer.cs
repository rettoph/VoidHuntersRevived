using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Options;

namespace VoidHuntersRevived.Domain.Entities.Common.Serialization
{
    [Service<ComponentSerializer>(ServiceLifetime.Scoped, ServiceRegistrationFlags.RequireAutoLoadAttribute)]
    public abstract class ComponentSerializer
    {
        public readonly Type Type;

        internal ComponentSerializer(Type type)
        {
            Type = type;
        }

        public abstract void Serialize(ref EntityWriter writer, in EntityId id, in GroupIndex groupIndex, EntitiesDB entitiesDB, in SerializationOptions options);
        public abstract void Deserialize(in VhId sourceId, in DeserializationOptions options, ref EntityReader reader, ref EntityInitializer initializer, in EntityId id);
    }

    public abstract class ComponentSerializer<TComponent> : ComponentSerializer
        where TComponent : unmanaged, IEntityComponent
    {
        public ComponentSerializer() : base(typeof(TComponent))
        {
        }

        public override void Serialize(ref EntityWriter writer, in EntityId id, in GroupIndex groupIndex, EntitiesDB entitiesDB, in SerializationOptions options)
        {
            var (components, _) = entitiesDB.QueryEntities<TComponent>(groupIndex.GroupID);
            ref var component = ref components[groupIndex.Index];

            this.Write(ref writer, id, component, in options);
        }
        public override void Deserialize(in VhId sourceId, in DeserializationOptions options, ref EntityReader reader, ref EntityInitializer initializer, in EntityId id)
        {
            initializer.Init<TComponent>(this.Read(in options, ref reader, in id));
        }

        protected abstract void Write(ref EntityWriter writer, in EntityId entityId, in TComponent instance, in SerializationOptions options);
        protected abstract TComponent Read(in DeserializationOptions options, ref EntityReader reader, in EntityId id);
    }

    public abstract class NotImplementedComponentSerializer<TComponent> : ComponentSerializer<TComponent>
        where TComponent : unmanaged, IEntityComponent
    {
        protected override TComponent Read(in DeserializationOptions options, ref EntityReader reader, in EntityId id)
        {
            throw new NotImplementedException();
        }

        protected override void Write(ref EntityWriter writer, in EntityId id, in TComponent instance, in SerializationOptions options)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class RawComponentSerializer<TComponent> : ComponentSerializer<TComponent>
        where TComponent : unmanaged, IEntityComponent
    {
        protected override unsafe TComponent Read(in DeserializationOptions options, ref EntityReader reader, in EntityId id)
        {
            return reader.Read<TComponent>();
        }

        protected override unsafe void Write(ref EntityWriter writer, in EntityId id, in TComponent instance, in SerializationOptions options)
        {
            writer.Write(instance);
        }
    }
}
