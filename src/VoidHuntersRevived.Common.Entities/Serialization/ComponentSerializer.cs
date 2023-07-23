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

        public abstract void Serialize(IEntityService entities, EntityWriter writer, uint sourceIndex, ExclusiveGroupStruct groupId, EntitiesDB entitiesDB);
        public abstract void Deserialize(IEntityService entities, EntityReader reader, ref EntityInitializer initializer);
    }

    public class ComponentSerializer<T> : ComponentSerializer
        where T : unmanaged, IEntityComponent
    {
        private Action<IEntityService, EntityWriter, T> _writer;
        private Func<IEntityService, EntityReader, T> _reader;

        public ComponentSerializer(Action<IEntityService, EntityWriter, T> writer, Func<IEntityService, EntityReader, T> reader) : base(typeof(T))
        {
            _writer = writer;
            _reader = reader;
        }

        public override void Serialize(IEntityService entities, EntityWriter writer, uint sourceIndex, ExclusiveGroupStruct groupId, EntitiesDB entitiesDB)
        {
            var (components, _) = entitiesDB.QueryEntities<T>(groupId);
            ref var component = ref components[sourceIndex];

            _writer(entities, writer, component);
        }

        public override void Deserialize(IEntityService entities, EntityReader reader, ref EntityInitializer initializer)
        {
            initializer.Init<T>(_reader(entities, reader));
        }


        public static ComponentSerializer<T> Default => new ComponentSerializer<T>(RawSerialize, RawDeserialize);

        private static unsafe void RawSerialize(IEntityService entities, EntityWriter writer, T component)
        {
            byte* pBytes = (byte*)&component;
            var span = new ReadOnlySpan<byte>(pBytes, sizeof(T));

            writer.Write(span);
        }
        private unsafe static T RawDeserialize(IEntityService entities, EntityReader reader)
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
