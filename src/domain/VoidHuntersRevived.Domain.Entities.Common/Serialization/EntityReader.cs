using Serilog;
using Svelto.Common;
using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Common.Serialization
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
            if (_loaded.Id.Value == data.Id.Value)
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

        internal EntityId Deserialize(VhId sourceId, EntityData data, DeserializationOptions options, InstanceEntityInitializerDelegate initializer)
        {
            VhId vhid = this.InternalDeserialize(sourceId, data, 0, options, initializer);

            for (uint i = 0; i < data.Positions.Length; i++)
            {
                this.InternalDeserialize(sourceId, data, data.Positions[i], options, initializer);
            }

            return _entities.GetId(vhid);
        }

        internal EntityId Deserialize(VhId sourceId, EntityData data, DeserializationOptions options, InstanceEntityInitializerDelegate initializer, EntityInitializerDelegate rootInitializer)
        {
            VhId vhid = this.InternalDeserialize(sourceId, data, 0, options, initializer, rootInitializer);

            for (uint i = 0; i < data.Positions.Length; i++)
            {
                this.InternalDeserialize(sourceId, data, data.Positions[i], options, initializer);
            }

            return _entities.GetId(vhid);
        }

        internal EntityId Deserialize(VhId sourceId, EntityData data, DeserializationOptions options, InstanceEntityInitializerDelegate initializer, InstanceEntityInitializerDelegate rootInitializer)
        {
            VhId vhid = this.InternalDeserialize(sourceId, data, 0, options, initializer, rootInitializer);

            for (uint i = 0; i < data.Positions.Length; i++)
            {
                this.InternalDeserialize(sourceId, data, data.Positions[i], options, initializer);
            }

            return _entities.GetId(vhid);
        }

        private VhId InternalDeserialize(VhId sourceId, EntityData data, long position, DeserializationOptions options, InstanceEntityInitializerDelegate initializerDelegate)
        {
            this.Load(data, position);
            VhId vhid = this.ReadVhId(options.Seed);
            Id<IEntityType> typeId = this.ReadStruct<Id<IEntityType>>();
            IEntityType type = _types.GetById(typeId);

            _logger.Verbose("{ClassName}::{MethodName} - Preparing to deserialize {EntityId} of type {EntityType} with seed {seed}", nameof(EntityReader), nameof(InternalDeserialize), vhid.Value, typeId.Value, options.Seed.Value);

            _entities.Spawn(sourceId, type, vhid, (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
            {
                this.Load(data, position + EntityReader.EntityHeaderSize);
                entities.GetDescriptorEngine(type.Descriptor.Id).Deserialize(in sourceId, in options, this, ref initializer, in id);

                initializerDelegate(ref initializer, in id);
            });

            return vhid;
        }

        private VhId InternalDeserialize(VhId sourceId, EntityData data, long position, DeserializationOptions options, InstanceEntityInitializerDelegate initializerDelegate, EntityInitializerDelegate rootInitializerDelegate)
        {
            this.Load(data, position);
            VhId vhid = this.ReadVhId(options.Seed);
            Id<IEntityType> typeId = this.ReadStruct<Id<IEntityType>>();
            IEntityType type = _types.GetById(typeId);

            _logger.Verbose("{ClassName}::{MethodName} - Preparing to deserialize {EntityId} of type {EntityType} with seed {seed}", nameof(EntityReader), nameof(InternalDeserialize), vhid.Value, typeId.Value, options.Seed.Value);

            _entities.Spawn(sourceId, type, vhid, (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
            {
                this.Load(data, position + EntityReader.EntityHeaderSize);
                entities.GetDescriptorEngine(type.Descriptor.Id).Deserialize(in sourceId, in options, this, ref initializer, in id);

                rootInitializerDelegate(entities, ref initializer, in id);
                initializerDelegate(ref initializer, in id);
            });

            return vhid;
        }

        private VhId InternalDeserialize(VhId sourceId, EntityData data, long position, DeserializationOptions options, InstanceEntityInitializerDelegate initializerDelegate, InstanceEntityInitializerDelegate rootInitializerDelegate)
        {
            this.Load(data, position);
            VhId vhid = this.ReadVhId(options.Seed);
            Id<IEntityType> typeId = this.ReadStruct<Id<IEntityType>>();
            IEntityType type = _types.GetById(typeId);

            _logger.Verbose("{ClassName}::{MethodName} - Preparing to deserialize {EntityId} of type {EntityType} with seed {seed}", nameof(EntityReader), nameof(InternalDeserialize), vhid.Value, typeId.Value, options.Seed.Value);

            _entities.Spawn(sourceId, type, vhid, (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
            {
                this.Load(data, position + EntityReader.EntityHeaderSize);
                entities.GetDescriptorEngine(type.Descriptor.Id).Deserialize(in sourceId, in options, this, ref initializer, in id);

                rootInitializerDelegate(ref initializer, in id);
                initializerDelegate(ref initializer, in id);
            });

            return vhid;
        }
    }
}
