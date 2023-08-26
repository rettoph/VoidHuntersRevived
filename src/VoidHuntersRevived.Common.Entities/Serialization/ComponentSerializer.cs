using Guppy.Attributes;
using Guppy.Enums;
using Svelto.ECS;
using System;
using System.Collections.Generic;
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

        public abstract void Serialize(EntityWriter writer, uint sourceIndex, ExclusiveGroupStruct groupId, EntitiesDB entitiesDB);
        public abstract void Deserialize(EntityReader reader, ref EntityInitializer initializer, in EntityId id);
    }

    [Service(ServiceLifetime.Scoped, null, true)]
    public abstract class ComponentSerializer<TComponent> : ComponentSerializer
        where TComponent : unmanaged, IEntityComponent
    {
        public ComponentSerializer() : base(typeof(TComponent))
        {
        }

        public override void Serialize(EntityWriter writer, uint sourceIndex, ExclusiveGroupStruct groupId, EntitiesDB entitiesDB)
        {
            var (components, _) = entitiesDB.QueryEntities<TComponent>(groupId);
            ref var component = ref components[sourceIndex];

            this.Write(writer, component);
        }
        public override void Deserialize(EntityReader reader, ref EntityInitializer initializer, in EntityId id)
        {
            initializer.Init<TComponent>(this.Read(reader, id));
        }

        protected abstract void Write(EntityWriter writer, TComponent instance);
        protected abstract TComponent Read(EntityReader reader, EntityId id);
    }

    public class DefaultComponentSerializer<T> : ComponentSerializer
        where T : unmanaged, IEntityComponent
    {
        private Action<EntityWriter, T> _writer;
        private Func<EntityReader, EntityId, T> _reader;

        public DefaultComponentSerializer(Action<EntityWriter, T> writer, Func<EntityReader, EntityId, T> reader) : base(typeof(T))
        {
            _writer = writer;
            _reader = reader;
        }

        public override void Serialize(EntityWriter writer, uint sourceIndex, ExclusiveGroupStruct groupId, EntitiesDB entitiesDB)
        {
            var (components, _) = entitiesDB.QueryEntities<T>(groupId);
            ref var component = ref components[sourceIndex];

            _writer(writer, component);
        }

        public override void Deserialize(EntityReader reader, ref EntityInitializer initializer, in EntityId id)
        {
            initializer.Init<T>(_reader(reader, id));
        }


        public static DefaultComponentSerializer<T> Default => new DefaultComponentSerializer<T>(RawSerialize, RawDeserialize);

        private static unsafe void RawSerialize(EntityWriter writer, T component)
        {
            byte* pBytes = (byte*)&component;
            var span = new ReadOnlySpan<byte>(pBytes, sizeof(T));

            writer.Write(span);
        }
        private unsafe static T RawDeserialize(EntityReader reader, EntityId id)
        {
            Span<byte> span = stackalloc byte[sizeof(T)];
            reader.Read(span);
            fixed (byte* pBytes = &span[0])
            {
                T* components = (T*)&pBytes[0];
                return components[0];
            }
        }
    }
}
