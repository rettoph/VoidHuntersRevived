using Svelto.Common;
using Svelto.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VoidHuntersRevived.Common.Entities.Serialization
{
    public class EntityReader : BinaryReader
    {
        private EntityData _loaded;

        public bool Busy { get; set; }
        public VhId Seed { get; private set; }
        public VhId Injection { get; private set; }

        public EntityReader() : base(new MemoryStream())
        {
            _loaded = new EntityData(VhId.Empty, Array.Empty<byte>());
        }

        public void Load(VhId seed, EntityData data, VhId injection)
        {
            if (this.Busy)
            {
                throw new Exception();
            }

            if (_loaded.Id.Value != data.Id.Value)
            {
                this.BaseStream.Position = 0;
                this.BaseStream.Write(data.Bytes, 0, data.Bytes.Length);

                _loaded = data;
            }

            this.BaseStream.Position = 0;
            this.BaseStream.Flush();

            this.Seed = seed;
            this.Injection = injection;
        }
        public void Load(EntityReaderState state)
        {
            if (_loaded.Id.Value != state.Data.Id.Value)
            {
                if (this.Busy)
                {
                    throw new Exception();
                }

                this.BaseStream.Position = state.Position;
                this.BaseStream.Write(state.Data.Bytes, state.Position, state.Data.Bytes.Length - state.Position);

                _loaded = state.Data;
            }

            this.BaseStream.Position = state.Position;
            this.Seed = state.Seed;
            this.Injection = state.Injection;
            this.Busy = true;
        }

        public EntityReaderState GetState()
        {
            return new EntityReaderState(_loaded!, this.Seed, (int)this.BaseStream.Position, this.Injection);
        }

        /// <summary>
        /// Read and seed a VhId value
        /// </summary>
        /// <returns></returns>
        public VhId ReadVhId()
        {
            return this.Seed.Create(this.ReadStruct<VhId>());
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

        public NativeDynamicArrayCast<T> ReadNativeDynamicArray<T>(Func<EntityReader, T> reader)
            where T : unmanaged
        {
            int count = this.ReadInt32();
            NativeDynamicArrayCast<T> native = new NativeDynamicArrayCast<T>((uint)count, Allocator.Persistent);

            for (int i = 0; i < count; i++)
            {
                native.Set(i, reader(this));
            }

            return native;
        }

        public NativeDynamicArrayCast<T> ReadNativeDynamicArray<T>()
            where T : unmanaged
        {
            return this.ReadNativeDynamicArray<T>(DefaultNativeDynamicArrayItemReader<T>);
        }

        private static T DefaultNativeDynamicArrayItemReader<T>(EntityReader reader)
            where T : unmanaged
        {
            return reader.ReadStruct<T>();
        }
    }
}
