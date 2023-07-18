﻿using Svelto.Common;
using Svelto.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VoidHuntersRevived.Common.Entities.Serialization
{
    public class EntityReader : BinaryReader
    {
        private EntityData _loaded;

        public bool Busy;

        public EntityReader() : base(new MemoryStream())
        {
            _loaded = new EntityData(VhId.Empty, Array.Empty<byte>());
        }

        public void Load(EntityData data)
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

            this.Busy = true;
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
            this.Busy = true;
        }

        public EntityReaderState GetState(in VhId seed)
        {
            return new EntityReaderState(_loaded!, seed, (int)this.BaseStream.Position);
        }

        public VhId ReadVhId(VhId seed)
        {
            return seed.Create(this.ReadUnmanaged<VhId>());
        }

        public unsafe T ReadUnmanaged<T>()
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
            return reader.ReadUnmanaged<T>();
        }
    }
}
