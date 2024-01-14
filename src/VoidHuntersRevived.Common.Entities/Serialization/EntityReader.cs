using Serilog;
using Svelto.Common;
using Svelto.DataStructures;
using Svelto.ECS;
using System.Reflection.PortableExecutable;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Entities.Options;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities.Serialization
{
    public class EntityReader : BinaryReader
    {
        private static unsafe long EntityHeaderSize = sizeof(VhId) + sizeof(Id<IEntityType>);

        private readonly IEntityTypeService _types;
        private readonly IEntityService _entities;
        private readonly ILogger _logger;

        private EntityData _loaded;

        public EntityReader(IEntityTypeService types, IEntityService entities, ILogger logger) : base(new MemoryStream())
        {
            _loaded = EntityData.Default;
            _types = types;
            _entities = entities;
            _logger = logger;
        }

        public void Load(EntityData data, long position)
        {
            if(_loaded.Id.Value == data.Id.Value)
            {
                this.BaseStream.Position = position;
                return;
            }

            this.BaseStream.Position = 0;
            this.BaseStream.Write(data.Bytes, 0, data.Bytes.Length);
            this.BaseStream.Flush();

            this.BaseStream.Position = position;

            _loaded = data;
        }

        /// <summary>
        /// Read and seed a VhId value
        /// </summary>
        /// <returns></returns>
        public VhId ReadVhId(VhId seed)
        {
            return seed.Create(this.ReadStruct<VhId>());
        }

        /// <summary>
        /// Read and return a bool
        /// </summary>
        /// <returns></returns>
        public bool ReadIf()
        {
            return this.ReadBoolean();
        }

        /// <summary>
        /// Read a raw value directly from the memor stream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public unsafe T ReadStruct<T>()
            where T : unmanaged
        {
            Span<byte> bytes = stackalloc byte[sizeof(T)];
            this.Read(bytes);

            fixed (byte* pbytes = &bytes[0])
            {
                T* value = (T*)&pbytes[0];

                return value[0];
            }
        }

        public NativeDynamicArrayCast<T> ReadNativeDynamicArray<T>(Func<DeserializationOptions, EntityReader, T> reader, in DeserializationOptions options)
            where T : unmanaged
        {
            int count = this.ReadInt32();
            NativeDynamicArrayCast<T> native = new NativeDynamicArrayCast<T>((uint)count, Allocator.Persistent);

            for (int i = 0; i < count; i++)
            {
                native.Set(i, reader(options, this));
            }

            return native;
        }

        public NativeDynamicArrayCast<T> ReadNativeDynamicArray<T>()
            where T : unmanaged
        {
            return this.ReadNativeDynamicArray<T>(DefaultNativeDynamicArrayItemReader<T>, default);
        }

        private static T DefaultNativeDynamicArrayItemReader<T>(DeserializationOptions options, EntityReader reader)
            where T : unmanaged
        {
            return reader.ReadStruct<T>();
        }

        internal EntityId Deserialize(VhId sourceId, EntityData data, DeserializationOptions options, EntityInitializerDelegate? initializerDelegate)
        {
            for(int i = data.Positions.Length - 1; i > -1; i--)
            {
                this.InternalDeserialize(sourceId, data, data.Positions[i], options, null);
            }

            VhId vhid = this.InternalDeserialize(sourceId, data, 0, options, initializerDelegate);

            return _entities.GetId(vhid);
        }

        private VhId InternalDeserialize(VhId sourceId, EntityData data, long position, DeserializationOptions options, EntityInitializerDelegate? initializerDelegate)
        {
            this.Load(data, position);
            VhId vhid = this.ReadVhId(options.Seed);
            Id<IEntityType> typeId = this.ReadStruct<Id<IEntityType>>();
            IEntityType type = _types.GetById(typeId);

            _logger.Verbose("{ClassName}::{MethodName} - Preparing to deserialize {EntityId} of type {EntityType} with seed {seed}", nameof(EntityReader), nameof(InternalDeserialize), vhid.Value, typeId.Value, options.Seed.Value);

            _entities.Spawn(sourceId, type, vhid, options.TeamId, (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
            {
                this.Load(data, position + EntityReader.EntityHeaderSize);
                entities.GetDescriptorEngine(type.Descriptor.Id).Deserialize(in sourceId, in options, this, ref initializer, in id);

                initializerDelegate?.Invoke(entities, ref initializer, in id);
            });

            return vhid;
        }
    }
}
