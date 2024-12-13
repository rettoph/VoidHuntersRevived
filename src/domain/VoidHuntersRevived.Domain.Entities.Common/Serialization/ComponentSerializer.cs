using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Options;

namespace VoidHuntersRevived.Domain.Entities.Common.Serialization
{
    public abstract class ComponentSerializer<TComponent> : IComponentSerializer
        where TComponent : unmanaged, IEntityComponent
    {
        public Type Type => typeof(TComponent);

        public ComponentSerializer()
        {
        }

        public virtual void Serialize(ref EntityWriter writer, in Entity entity, EntitiesDB entitiesDB, in SerializationOptions options)
        {
            ref var component = ref entitiesDB.QueryEntityByIndex<TComponent>(entity.Index, entity.Group);

            this.Write(ref writer, in entity, component, in options);
        }
        public virtual void Deserialize(in VhId sourceId, in DeserializationOptions options, ref EntityReader reader, ref EntityInitializer initializer, in EntityId id)
        {
            initializer.Init<TComponent>(this.Read(in options, ref reader, in id));
        }

        protected abstract void Write(ref EntityWriter writer, in Entity entity, in TComponent instance, in SerializationOptions options);
        protected abstract TComponent Read(in DeserializationOptions options, ref EntityReader reader, in EntityId id);
    }

    public abstract class NotImplementedComponentSerializer<TComponent> : ComponentSerializer<TComponent>
        where TComponent : unmanaged, IEntityComponent
    {
        protected override TComponent Read(in DeserializationOptions options, ref EntityReader reader, in EntityId id)
        {
            throw new NotImplementedException();
        }

        protected override void Write(ref EntityWriter writer, in Entity entity, in TComponent instance, in SerializationOptions options)
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

        protected override unsafe void Write(ref EntityWriter writer, in Entity entity, in TComponent instance, in SerializationOptions options)
        {
            writer.Write(instance);
        }
    }
}
