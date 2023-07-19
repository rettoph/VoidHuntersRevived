using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Serialization
{
    public abstract class ComponentSerializer
    {
        public readonly Type Type;

        internal ComponentSerializer(Type type)
        {
            Type = type;
        }

        public abstract void Serialize(EntityWriter writer, uint sourceIndex, ExclusiveGroupStruct groupId, EntitiesDB entities);
        public abstract void Deserialize(EntityReader reader, ref EntityInitializer initializer);
    }

    public class ComponentSerializer<T> : ComponentSerializer
        where T : unmanaged, IEntityComponent
    {
        private Action<EntityWriter, T> _writer;
        private Func<EntityReader, T> _reader;

        public ComponentSerializer(Action<EntityWriter, T> writer, Func<EntityReader, T> reader) : base(typeof(T))
        {
            _writer = writer;
            _reader = reader;
        }

        public override void Serialize(EntityWriter writer, uint sourceIndex, ExclusiveGroupStruct groupId, EntitiesDB entities)
        {
            var (components, _) = entities.QueryEntities<T>(groupId);
            ref var component = ref components[sourceIndex];

            _writer(writer, component);
        }

        public override void Deserialize(EntityReader reader, ref EntityInitializer initializer)
        {
            initializer.Init<T>(_reader(reader));
        }


        public static ComponentSerializer<T> Default => new ComponentSerializer<T>(RawSerialize, RawDeserialize);

        private static unsafe void RawSerialize(EntityWriter writer, T component)
        {
            byte* pBytes = (byte*)&component;
            var span = new ReadOnlySpan<byte>(pBytes, sizeof(T));

            writer.Write(span);
        }
        private unsafe static T RawDeserialize(EntityReader reader)
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
