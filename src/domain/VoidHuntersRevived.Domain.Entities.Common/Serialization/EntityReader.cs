using Serilog;
using Svelto.Common;
using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Common.Serialization
{
    public class EntityReader : BinaryReader
    {
        private static unsafe long EntityHeaderSize = sizeof(VhId) + sizeof(Id<IEntityType>);

        private readonly IEntityTypeProviderService _entityTypeProviderService;
        private readonly IEntityQueryService _entityQueryService;
        private readonly IEntitySpawnService _entitySpawnService;
        private readonly ILogger _logger;

        private EntityData _loaded;

        public EntityReader(
            IEntityTypeProviderService entityTypeProviderService,
            IEntityQueryService entityQueryService,
            IEntitySpawnService entitySpawnService,
            ILogger logger) : base(new MemoryStream())
        {
            _loaded = EntityData.Default;
            _entityTypeProviderService = entityTypeProviderService;
            _entityQueryService = entityQueryService;
            _entitySpawnService = entitySpawnService;
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

        internal EntityId Deserialize(VhId sourceId, EntityData data, DeserializationOptions options, EntityInitializerDelegate initializer)
        {
            VhId vhid = this.InternalDeserialize(sourceId, data, 0, options, initializer);

            for (uint i = 0; i < data.Positions.Length; i++)
            {
                this.InternalDeserialize(sourceId, data, data.Positions[i], options, initializer);
            }

            return _entityQueryService.GetId(vhid);
        }

        internal EntityId Deserialize(VhId sourceId, EntityData data, DeserializationOptions options, EntityInitializerDelegate initializer, EntityInitializerDelegate rootInitializer)
        {
            VhId vhid = this.InternalDeserialize(sourceId, data, 0, options, initializer, rootInitializer);

            for (uint i = 0; i < data.Positions.Length; i++)
            {
                this.InternalDeserialize(sourceId, data, data.Positions[i], options, initializer);
            }

            return _entityQueryService.GetId(vhid);
        }

        private VhId InternalDeserialize(VhId sourceId, EntityData data, long position, DeserializationOptions options, EntityInitializerDelegate initializerDelegate)
        {
            this.Load(data, position);
            VhId vhid = this.ReadVhId(options.Seed);
            Key<IEntityType> entityTypeKey = Key<IEntityType>.GetById(this.ReadStruct<VhId>());

            _logger.Verbose("{ClassName}::{MethodName} - Preparing to deserialize {EntityId} of type {EntityType} with seed {seed}", nameof(EntityReader), nameof(InternalDeserialize), vhid.Value, entityTypeKey, options.Seed.Value);

            _entitySpawnService.Spawn(sourceId, entityTypeKey, vhid, (IEntityService entities, IEntityType entityType, EntityId id, ref EntityInitializer initializer) =>
            {
                this.Load(data, position + EntityReader.EntityHeaderSize);
                _entityTypeProviderService.GetByKey(entityTypeKey).DeserializeInstanceEntity(in sourceId, in options, this, ref initializer, in id);

                initializerDelegate(entities, entityType, id, ref initializer);
            });

            return vhid;
        }

        private VhId InternalDeserialize(VhId sourceId, EntityData data, long position, DeserializationOptions options, EntityInitializerDelegate initializerDelegate, EntityInitializerDelegate rootInitializerDelegate)
        {
            this.Load(data, position);
            VhId vhid = this.ReadVhId(options.Seed);
            Key<IEntityType> entityTypeKey = Key<IEntityType>.GetById(this.ReadStruct<VhId>());

            _logger.Verbose("{ClassName}::{MethodName} - Preparing to deserialize {EntityId} of type {EntityType} with seed {seed}", nameof(EntityReader), nameof(InternalDeserialize), vhid.Value, entityTypeKey, options.Seed.Value);

            _entitySpawnService.Spawn(sourceId, entityTypeKey, vhid, (IEntityService entities, IEntityType entityType, EntityId id, ref EntityInitializer initializer) =>
            {
                this.Load(data, position + EntityReader.EntityHeaderSize);
                _entityTypeProviderService.GetByKey(entityTypeKey).DeserializeInstanceEntity(in sourceId, in options, this, ref initializer, in id);

                rootInitializerDelegate(entities, entityType, id, ref initializer);
                initializerDelegate(entities, entityType, id, ref initializer);
            });

            return vhid;
        }
    }
}
