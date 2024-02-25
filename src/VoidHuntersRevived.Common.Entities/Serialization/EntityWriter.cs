using Serilog;
using Svelto.DataStructures;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Options;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities.Serialization
{
    public sealed class EntityWriter : BinaryWriter
    {
        private readonly Stack<EntityId> _nested;
        private readonly List<long> _positions;
        private readonly IEntityService _entities;
        private readonly ILogger _logger;

        public EntityWriter(IEntityService entities, ILogger logger) : base(new MemoryStream())
        {
            _nested = new Stack<EntityId>();
            _positions = new List<long>();
            _entities = entities;
            _logger = logger;
        }

        public unsafe void WriteStruct<T>(T value)
            where T : unmanaged
        {
            byte* pBytes = (byte*)&value;
            var span = new ReadOnlySpan<byte>(pBytes, sizeof(T));

            this.Write(span);
        }

        public void Write(VhId vhid)
        {
            this.WriteStruct(vhid);
        }

        public bool WriteIf(bool value)
        {
            this.Write(value);

            return value;
        }

        public void WriteNativeDynamicArray<T>(NativeDynamicArrayCast<T> native, Action<EntityWriter, T, SerializationOptions> writer, in SerializationOptions options)
            where T : unmanaged
        {
            this.Write(native.count);

            for (int i = 0; i < native.count; i++)
            {
                writer(this, native[i], options);
            }
        }

        public void WriteNativeDynamicArray<T>(NativeDynamicArrayCast<T> native)
            where T : unmanaged
        {
            this.WriteNativeDynamicArray<T>(native, DefaultNativeDynamicArrayItemWriter<T>, SerializationOptions.Default);
        }

        private static void DefaultNativeDynamicArrayItemWriter<T>(EntityWriter writer, T item, SerializationOptions options)
            where T : unmanaged
        {
            writer.WriteStruct<T>(item);
        }

        public void Push(EntityId id)
        {
            _nested.Push(id);
        }

        internal EntityData Serialize(EntityId id, SerializationOptions options)
        {
            _nested.Clear();
            _positions.Clear();
            this.BaseStream.Position = 0;

            this.InternalSerialize(id, options);
            while (_nested.TryPop(out EntityId nestedId))
            {
                _positions.Add(this.BaseStream.Position);
                this.InternalSerialize(nestedId, options);
            }

            long[] positions = Array.Empty<long>();
            if (_positions.Count > 0)
            {
                positions = _positions.ToArray();
            }

            byte[] bytes = new byte[this.BaseStream.Position];

            this.BaseStream.Position = 0;
            this.BaseStream.Read(bytes, 0, bytes.Length);

            return new EntityData(id.VhId, positions, bytes);
        }

        private void InternalSerialize(EntityId id, SerializationOptions options)
        {
            Id<IEntityType> typeId = _entities.QueryById<Id<IEntityType>>(id, out GroupIndex groupIndex);
            Id<VoidHuntersEntityDescriptor> descriptorId = _entities.QueryByGroupIndex<Id<VoidHuntersEntityDescriptor>>(in groupIndex);

            _logger.Verbose("{ClassName}::{MethodName} - Preparing to serialize {EntityId} of type {EntityType}", nameof(EntityWriter), nameof(InternalSerialize), id.VhId, typeId.Value);

            this.Write(id.VhId);
            this.WriteStruct(typeId);
            _entities.GetDescriptorEngine(descriptorId).Serialize(this, in groupIndex, in options);
        }
    }
}
