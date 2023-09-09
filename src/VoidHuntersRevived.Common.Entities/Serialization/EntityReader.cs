using Svelto.Common;
using Svelto.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Options;
using VoidHuntersRevived.Common.Entities.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VoidHuntersRevived.Common.Entities.Serialization
{
    public class EntityReader : BinaryReader
    {
        private EntityData _loaded;

        public EntityReader() : base(new MemoryStream())
        {
            _loaded = new EntityData(VhId.Empty, Array.Empty<byte>());
        }

        public void Load(EntityData data)
        {
            if (_loaded.Id.Value != data.Id.Value)
            {
                this.BaseStream.Position = 0;
                this.BaseStream.Write(data.Bytes, 0, data.Bytes.Length);

                _loaded = data;
            }

            this.BaseStream.Position = 0;
            this.BaseStream.Flush();
        }
        public void Load(EntityReaderState state)
        {
            if (_loaded.Id.Value != state.Data.Id.Value)
            {
                this.BaseStream.Position = state.Position;
                this.BaseStream.Write(state.Data.Bytes, state.Position, state.Data.Bytes.Length - state.Position);

                _loaded = state.Data;
            }

            this.BaseStream.Position = state.Position;
        }

        public EntityReaderState GetState()
        {
            return new EntityReaderState(_loaded!, (int)this.BaseStream.Position);
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

            fixed(byte* pbytes = &bytes[0])
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
    }
}
